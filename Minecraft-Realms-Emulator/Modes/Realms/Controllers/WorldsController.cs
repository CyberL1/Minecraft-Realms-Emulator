using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Modes.Realms.Helpers;
using Minecraft_Realms_Emulator.Requests;
using Minecraft_Realms_Emulator.Responses;
using Newtonsoft.Json;
using Semver;
using System.Net;
using System.Net.Sockets;

namespace Minecraft_Realms_Emulator.Modes.Realms.Controllers
{
    [Route("modes/realms/[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class WorldsController : ControllerBase
    {
        private readonly DataContext _context;

        public WorldsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ServersResponse>> GetWorlds()
        {
            string cookie = Request.Headers.Cookie;

            string playerUUID = cookie.Split(";")[0].Split(":")[2];
            string playerName = cookie.Split(";")[1].Split("=")[1];
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var ownedWorlds = await _context.Worlds.Where(w => w.OwnerUUID == playerUUID).Include(w => w.Subscription).Include(w => w.Slots).ToListAsync();
            var memberWorlds = await _context.Players.Where(p => p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Include(p => p.World.Slots).Select(p => p.World).ToListAsync();

            List<WorldResponse> allWorlds = [];

            if (ownedWorlds.ToArray().Length == 0)
            {
                var world = new World
                {
                    Owner = playerName,
                    OwnerUUID = playerUUID,
                    Name = null,
                    Motd = null,
                    State = nameof(StateEnum.UNINITIALIZED),
                    WorldType = nameof(WorldTypeEnum.NORMAL),
                    MaxPlayers = 10,
                    MinigameId = null,
                    MinigameName = null,
                    MinigameImage = null,
                    ActiveSlot = 1,
                    Member = false
                };

                ownedWorlds.Add(world);
                _context.Worlds.Add(world);

                _context.SaveChanges();
            }

            foreach (var world in ownedWorlds)
            {
                Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

                int versionsCompared = SemVersion.Parse(gameVersion, SemVersionStyles.OptionalPatch).ComparePrecedenceTo(SemVersion.Parse(activeSlot?.Version ?? gameVersion, SemVersionStyles.Any));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    MinigameId = world.MinigameId,
                    MinigameName = world.MinigameName,
                    MinigameImage = world.MinigameImage,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    ActiveVersion = activeSlot?.Version ?? gameVersion,
                    Compatibility = isCompatible
                };

                if (world.Subscription != null)
                {
                    response.DaysLeft = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days;
                    response.Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0;
                    response.ExpiredTrial = false;
                }

                allWorlds.Add(response);
            }

            foreach (var world in memberWorlds)
            {
                Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

                int versionsCompared = SemVersion.Parse(gameVersion, SemVersionStyles.OptionalPatch).ComparePrecedenceTo(SemVersion.Parse(activeSlot.Version, SemVersionStyles.OptionalPatch));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    MinigameId = world.MinigameId,
                    MinigameName = world.MinigameName,
                    MinigameImage = world.MinigameImage,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    DaysLeft = 0,
                    Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                    ExpiredTrial = false,
                    ActiveVersion = activeSlot.Version,
                    Compatibility = isCompatible
                };

                allWorlds.Add(response);
            }

            ServersResponse servers = new()
            {
                Servers = allWorlds
            };

            return Ok(servers);
        }

        [HttpGet("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<WorldResponse>> GetWorldById(int wId)
        {
            string cookie = Request.Headers.Cookie;
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var world = await _context.Worlds.Include(w => w.Players).Include(w => w.Subscription).Include(w => w.Slots).FirstOrDefaultAsync(w => w.Id == wId);

            Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

            List<SlotResponse> slots = [];

            foreach (var slot in world.Slots)
            {
                int versionsCompared = SemVersion.Parse(gameVersion, SemVersionStyles.OptionalPatch).ComparePrecedenceTo(SemVersion.Parse(slot.Version, SemVersionStyles.OptionalPatch));
                string compatibility = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                slots.Add(new SlotResponse()
                {
                    SlotId = slot.SlotId,
                    Options = JsonConvert.SerializeObject(new
                    {
                        slotName = slot.SlotName,
                        gameMode = slot.GameMode,
                        difficulty = slot.Difficulty,
                        spawnProtection = slot.SpawnProtection,
                        forceGameMode = slot.ForceGameMode,
                        pvp = slot.Pvp,
                        spawnAnimals = slot.SpawnAnimals,
                        spawnMonsters = slot.SpawnMonsters,
                        spawnNPCs = slot.SpawnNPCs,
                        commandBlocks = slot.CommandBlocks,
                        version = slot.Version,
                        compatibility
                    })
                });
            }

            var activeSlotOptions = JsonConvert.DeserializeObject<SlotOptionsResponse>(slots.Find(s => s.SlotId == activeSlot.SlotId).Options);

            WorldResponse response = new()
            {
                Id = world.Id,
                Owner = world.Owner,
                OwnerUUID = world.OwnerUUID,
                Name = world.Name,
                Motd = world.Motd,
                State = world.State,
                WorldType = world.WorldType,
                MaxPlayers = world.MaxPlayers,
                MinigameId = world.MinigameId,
                MinigameName = world.MinigameName,
                MinigameImage = world.MinigameImage,
                ActiveSlot = world.ActiveSlot,
                Slots = slots,
                Member = world.Member,
                Players = world.Players,
                DaysLeft = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days,
                Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                ExpiredTrial = false,
                ActiveVersion = activeSlotOptions.Version,
                Compatibility = activeSlotOptions.Compatibility
            };

            return response;
        }

        [HttpPost("{wId}/initialize")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<World>> Initialize(int wId, WorldCreateRequest body)
        {
            string cookie = Request.Headers.Cookie;
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);
            if (world.State != nameof(StateEnum.UNINITIALIZED))
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 401,
                    ErrorMsg = "World already initialized",
                };

                return StatusCode(401, errorResponse);
            }

            var subscription = new Subscription
            {
                StartDate = DateTime.UtcNow,
                SubscriptionType = nameof(SubscriptionTypeEnum.NORMAL)
            };

            world.Name = body.Name;
            world.Motd = body.Description;
            world.State = nameof(StateEnum.OPEN);
            world.Subscription = subscription;

            var config = new ConfigHelper(_context);
            var defaultServerAddress = config.GetSetting(nameof(SettingsEnum.DefaultServerAddress));

            static int FindFreeTcpPort()
            {
                TcpListener l = new(IPAddress.Loopback, 0);
                l.Start();
                int port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                return port;
            }

            var port = FindFreeTcpPort();

            var connection = new Connection
            {
                World = world,
                Address = $"{defaultServerAddress.Value}:{port}"
            };

            Slot slot = new()
            {
                World = world,
                SlotId = 1,
                SlotName = "",
                Version = gameVersion,
                GameMode = 0,
                Difficulty = 2,
                SpawnProtection = 0,
                ForceGameMode = false,
                Pvp = true,
                SpawnAnimals = true,
                SpawnMonsters = true,
                SpawnNPCs = true,
                CommandBlocks = false
            };

            new DockerHelper(world).CreateServer(port);

            _context.Worlds.Update(world);

            _context.Subscriptions.Add(subscription);
            _context.Connections.Add(connection);
            _context.Slots.Add(slot);

            _context.SaveChanges();

            return Ok(world);
        }

        [HttpPost("{wId}/reset")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public ActionResult<bool> Reset(int wId)
        {
            Console.WriteLine($"Resetting world {wId}");
            return Ok(true);
        }

        [HttpPut("{wId}/open")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<bool>> Open(int wId)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            new DockerHelper(world).StartServer();

            world.State = nameof(StateEnum.OPEN);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("{wId}/close")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<bool>> Close(int wId)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.FirstOrDefault(w => w.Id == wId);

            new DockerHelper(world).StopServer();

            world.State = nameof(StateEnum.CLOSED);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<bool>> UpdateWorld(int wId, WorldCreateRequest body)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            world.Name = body.Name;
            world.Motd = body.Description;

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}/slot/{sId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<bool>> UpdateSlotAsync(int wId, int sId, SlotOptionsRequest body)
        {
            var slots = await _context.Slots.Where(s => s.World.Id == wId).ToListAsync();
            var slot = slots.Find(s => s.SlotId == sId);

            slot.SlotName = body.SlotName;
            slot.GameMode = body.GameMode;
            slot.Difficulty = body.Difficulty;
            slot.SpawnProtection = body.SpawnProtection;
            slot.ForceGameMode = body.ForceGameMode;
            slot.Pvp = body.Pvp;
            slot.SpawnAnimals = body.SpawnAnimals;
            slot.SpawnMonsters = body.SpawnMonsters;
            slot.SpawnNPCs = body.SpawnNPCs;
            slot.CommandBlocks = body.CommandBlocks;

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("{wId}/slot/{sId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public ActionResult<bool> SwitchSlot(int wId, int sId)
        {
            var world = _context.Worlds.Find(wId);

            var slot = _context.Slots.Where(s => s.World.Id == wId).Where(s => s.SlotId == sId).Any();

            if (!slot)
            {
                string cookie = Request.Headers.Cookie;
                string gameVersion = cookie.Split(";")[2].Split("=")[1];

                _context.Slots.Add(new()
                {
                    World = world,
                    SlotId = sId,
                    SlotName = "",
                    Version = gameVersion,
                    GameMode = 0,
                    Difficulty = 2,
                    SpawnProtection = 0,
                    ForceGameMode = false,
                    Pvp = true,
                    SpawnAnimals = true,
                    SpawnMonsters = true,
                    SpawnNPCs = true,
                    CommandBlocks = false
                });

                _context.SaveChanges();
            }

            world.ActiveSlot = sId;
            _context.SaveChanges();

            return Ok(true);
        }

        [HttpGet("{wId}/backups")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<BackupsResponse>> GetBackups(int wId)
        {
            var backups = await _context.Backups.Where(b => b.World.Id == wId).ToListAsync();

            BackupsResponse worldBackups = new()
            {
                Backups = backups
            };

            return Ok(worldBackups);
        }

        [HttpGet("v1/{wId}/join/pc")]
        public ActionResult<Connection> Join(int wId)
        {
            var connection = _context.Connections.FirstOrDefault(x => x.World.Id == wId);

            return Ok(connection);
        }

        [HttpDelete("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        public ActionResult<bool> DeleteRealm(int wId)
        {
            var world = _context.Worlds.Find(wId);

            new DockerHelper(world).DeleteServer();

            _context.Worlds.Remove(world);
            _context.SaveChanges();

            return Ok(true);
        }
    }
}
