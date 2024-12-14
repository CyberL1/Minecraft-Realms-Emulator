using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Shared.Attributes;
using Minecraft_Realms_Emulator.Shared.Enums;
using Minecraft_Realms_Emulator.Shared.Helpers;
using Minecraft_Realms_Emulator.Shared.Data;
using Minecraft_Realms_Emulator.Shared.Entities;
using Minecraft_Realms_Emulator.Shared.Requests;
using Minecraft_Realms_Emulator.Shared.Responses;
using Newtonsoft.Json;

namespace Minecraft_Realms_Emulator.Modes.External
{
    [Route("modes/external/[controller]")]
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

            var ownedWorlds = await _context.Worlds.Where(w => w.OwnerUUID == playerUUID).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.Minigame).ToListAsync();
            var memberWorlds = await _context.Players.Where(p => p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Include(p => p.World.Slots).Include(p => p.World.Minigame).Select(p => p.World).ToListAsync();

            List<WorldResponse> allWorlds = [];

            if (ownedWorlds.ToArray().Length == 0 && new ConfigHelper(_context).GetSetting(nameof(SettingsEnum.AutomaticRealmsCreation)).Value)
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
                    Minigame = null,
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

                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(activeSlot?.Version ?? gameVersion));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = activeSlot?.GameMode ?? 0,
                    IsHardcore = activeSlot?.Difficulty == 3,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    ActiveVersion = activeSlot?.Version ?? gameVersion,
                    Compatibility = isCompatible
                };

                if (world.Minigame != null)
                {
                    response.MinigameId = world.Minigame.Id;
                    response.MinigameName = world.Minigame.Name;
                    response.MinigameImage = world.Minigame.Image;
                }

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

                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(activeSlot.Version));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = activeSlot.GameMode,
                    IsHardcore = activeSlot.Difficulty == 3,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    DaysLeft = 0,
                    Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                    ExpiredTrial = false,
                    ActiveVersion = activeSlot.Version,
                    Compatibility = isCompatible
                };

                if (world.Minigame != null)
                {
                    response.MinigameId = world.Minigame.Id;
                    response.MinigameName = world.Minigame.Name;
                    response.MinigameImage = world.Minigame.Image;
                }

                allWorlds.Add(response);
            }

            ServersResponse servers = new()
            {
                Servers = allWorlds
            };

            return Ok(servers);
        }

        [HttpGet("listUserWorldsOfType/any")]
        public async Task<ActionResult<ServersResponse>> GetWorldsSnapshot()
        {
            string cookie = Request.Headers.Cookie;

            string playerUUID = cookie.Split(";")[0].Split(":")[2];
            string playerName = cookie.Split(";")[1].Split("=")[1];
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var ownedWorlds = await _context.Worlds.Where(w => w.OwnerUUID == playerUUID || w.ParentWorld.OwnerUUID == playerUUID).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.Minigame).Include(w => w.ParentWorld).ToListAsync();
            var memberWorlds = await _context.Players.Where(p => p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Include(p => p.World.Slots).Include(p => p.World.ParentWorld).Include(p => p.World.Minigame).Select(p => p.World).ToListAsync();

            List<WorldResponse> allWorlds = [];

            foreach (var world in ownedWorlds)
            {
                Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(activeSlot?.Version ?? gameVersion));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = activeSlot?.GameMode ?? 0,
                    IsHardcore = activeSlot?.Difficulty == 3,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    ActiveVersion = activeSlot?.Version ?? gameVersion,
                    Compatibility = isCompatible
                };

                if (world.Minigame != null)
                {
                    response.MinigameId = world.Minigame.Id;
                    response.MinigameName = world.Minigame.Name;
                    response.MinigameImage = world.Minigame.Image;
                }

                if (world.Subscription != null)
                {
                    response.DaysLeft = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days;
                    response.Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0;
                    response.ExpiredTrial = false;
                }

                if (world.ParentWorld == null)
                {
                    response.ParentWorldId = -1;
                }

                if (world.ParentWorld != null)
                {
                    response.Owner = world.ParentWorld.Owner;
                    response.OwnerUUID = world.ParentWorld.OwnerUUID;
                    response.ParentWorldId = world.ParentWorld.Id;
                    response.ParentWorldName = world.ParentWorld.Name;
                }

                allWorlds.Add(response);
            }

            foreach (var world in memberWorlds)
            {
                Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(activeSlot.Version));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = activeSlot.GameMode,
                    IsHardcore = activeSlot.Difficulty == 3,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    DaysLeft = 0,
                    Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                    ExpiredTrial = false,
                    ActiveVersion = activeSlot.Version,
                    Compatibility = isCompatible
                };

                if (world.Minigame != null)
                {
                    response.MinigameId = world.Minigame.Id;
                    response.MinigameName = world.Minigame.Name;
                    response.MinigameImage = world.Minigame.Image;
                }

                if (world.ParentWorld == null)
                {
                    response.ParentWorldId = -1;
                }

                if (world.ParentWorld != null)
                {
                    response.Owner = world.ParentWorld.Owner;
                    response.OwnerUUID = world.ParentWorld.OwnerUUID;
                    response.ParentWorldId = world.ParentWorld.Id;
                    response.ParentWorldName = world.ParentWorld.Name;
                }

                allWorlds.Add(response);
            }

            ServersResponse servers = new()
            {
                Servers = allWorlds
            };

            return Ok(servers);
        }

        [HttpGet("listPrereleaseEligibleWorlds")]
        public async Task<ActionResult<ServersResponse>> GetPrereleaseWorlds()
        {
            string cookie = Request.Headers.Cookie;

            string playerUUID = cookie.Split(";")[0].Split(":")[2];
            string playerName = cookie.Split(";")[1].Split("=")[1];
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var ownedWorlds = await _context.Worlds.Where(w => w.ParentWorld != null && w.ParentWorld.OwnerUUID == playerUUID).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.ParentWorld).Include(w => w.Minigame).ToListAsync();
            var memberWorlds = await _context.Players.Where(p => p.World.ParentWorld != null && p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Include(p => p.World.Slots).Include(p => p.World.ParentWorld).Include(p => p.World.Minigame).Select(p => p.World).ToListAsync();

            List<WorldResponse> allWorlds = [];

            if (ownedWorlds.ToArray().Length == 0 && new ConfigHelper(_context).GetSetting(nameof(SettingsEnum.AutomaticRealmsCreation)).Value)
            {
                var parentWorld = _context.Worlds.FirstOrDefault(w => w.OwnerUUID == playerUUID && w.ParentWorld == null);

                if (parentWorld != null && parentWorld.State != nameof(StateEnum.UNINITIALIZED))
                {
                    var world = new World
                    {
                        Name = parentWorld.Name,
                        Motd = null,
                        State = nameof(StateEnum.UNINITIALIZED),
                        WorldType = nameof(WorldTypeEnum.NORMAL),
                        MaxPlayers = 10,
                        Minigame = null,
                        ActiveSlot = 1,
                        Member = false,
                        ParentWorld = parentWorld,
                    };

                    ownedWorlds.Add(world);
                    _context.Worlds.Add(world);

                    _context.SaveChanges();
                }
            }

            foreach (var world in ownedWorlds)
            {
                Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(activeSlot?.Version ?? gameVersion));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = activeSlot?.GameMode ?? 0,
                    IsHardcore = activeSlot?.Difficulty == 3,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    ActiveVersion = activeSlot?.Version ?? gameVersion,
                    Compatibility = isCompatible,
                    ParentWorldId = world.ParentWorld.Id,
                    ParentWorldName = world.ParentWorld.Name,
                };

                if (world.Minigame != null)
                {
                    response.MinigameId = world.Minigame.Id;
                    response.MinigameName = world.Minigame.Name;
                    response.MinigameImage = world.Minigame.Image;
                }

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

                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(activeSlot.Version));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = activeSlot.GameMode,
                    IsHardcore = activeSlot.Difficulty == 3,
                    State = world.State,
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot,
                    Member = world.Member,
                    Players = world.Players,
                    DaysLeft = 0,
                    Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                    ExpiredTrial = false,
                    ActiveVersion = activeSlot.Version,
                    Compatibility = isCompatible
                };

                if (world.Minigame != null)
                {
                    response.MinigameId = world.Minigame.Id;
                    response.MinigameName = world.Minigame.Name;
                    response.MinigameImage = world.Minigame.Image;
                }

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

            var world = await _context.Worlds.Include(w => w.Players).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.ParentWorld.Subscription).Include(w => w.Minigame).FirstOrDefaultAsync(w => w.Id == wId);

            if (world.State == nameof(StateEnum.UNINITIALIZED))
            {
                ErrorResponse error = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Initialize the world first"
                };

                return StatusCode(400, error);
            }

            Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

            List<SlotResponse> slots = [];

            foreach (var slot in world.Slots)
            {
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(slot.Version));
                string compatibility = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                slots.Add(new SlotResponse()
                {
                    SlotId = slot.SlotId,
                    Options = JsonConvert.SerializeObject(new
                    {
                        slotName = slot.SlotName,
                        gameMode = slot.GameMode,
                        hardcore = slot.Difficulty == 3,
                        difficulty = slot.Difficulty,
                        spawnProection = slot.SpawnProtection,
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

            if (world.ParentWorld != null)
            {
                world.Subscription = world.ParentWorld.Subscription;
            }

            WorldResponse response = new()
            {
                Id = world.Id,
                Owner = world.Owner,
                OwnerUUID = world.OwnerUUID,
                Name = world.Name,
                Motd = world.Motd,
                GameMode = activeSlot.GameMode,
                IsHardcore = activeSlot.GameMode == 3,
                State = world.State,
                WorldType = world.WorldType,
                MaxPlayers = world.MaxPlayers,
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

            if (world.Minigame != null)
            {
                response.MinigameId = world.Minigame.Id;
                response.MinigameName = world.Minigame.Name;
                response.MinigameImage = world.Minigame.Image;
            }

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

            var connection = new Connection
            {
                World = world,
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

            _context.Worlds.Update(world);

            _context.Subscriptions.Add(subscription);
            _context.Connections.Add(connection);
            _context.Slots.Add(slot);

            _context.SaveChanges();

            return Ok(world);
        }

        [HttpPost("{wId}/createPrereleaseRealm")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<World>> CreatePrereleaseRealms(int wId)
        {
            string cookie = Request.Headers.Cookie;
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            if (world.ParentWorld.State == nameof(StateEnum.UNINITIALIZED))
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 401,
                    ErrorMsg = "You must initialize release world first",
                };

                return StatusCode(401, errorResponse);
            }

            if (world.State != nameof(StateEnum.UNINITIALIZED))
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 401,
                    ErrorMsg = "A prerealease realm is already created for this world",
                };

                return StatusCode(401, errorResponse);
            }

            return Ok(world);
        }

        [HttpPost("{wId}/reset")]
        [CheckForWorld]
        [CheckRealmOwner]
        public ActionResult<bool> Reset(int wId)
        {
            Console.WriteLine($"Resetting world {wId}");
            return Ok(true);
        }

        [HttpPut("{wId}/open")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<bool>> Open(int wId)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            world.State = nameof(StateEnum.OPEN);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("{wId}/close")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<bool>> Close(int wId)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.FirstOrDefault(w => w.Id == wId);

            world.State = nameof(StateEnum.CLOSED);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<(bool, ErrorResponse)>> UpdateWorld(int wId, WorldCreateRequest body)
        {
            if (body.Name.Length > 32)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "World name cannot exceed 32 characters"
                };

                return BadRequest(errorResponse);
            }

            if (body.Description?.Length > 32)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "World description cannot exceed 32 characters"
                };

                return BadRequest(errorResponse);
            }

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
        public async Task<ActionResult<(bool, ErrorResponse)>> UpdateSlot(int wId, int sId, SlotOptionsRequest body)
        {
            if (body.SlotName.Length > 10)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Slot name cannot exceed 10 characters"
                };

                return BadRequest(errorResponse);
            }

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
        public ActionResult<bool> SwitchSlot(int wId, int sId)
        {
            var world = _context.Worlds.Include(w => w.Minigame).FirstOrDefault(w => w.Id == wId);
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
            world.Minigame = null;
            world.WorldType = nameof(WorldTypeEnum.NORMAL);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpGet("{wId}/backups")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<BackupsResponse>> GetBackups(int wId)
        {
            var backups = await _context.Backups.Where(b => b.Slot.World.Id == wId).ToListAsync();

            BackupsResponse worldBackups = new()
            {
                Backups = backups
            };

            return Ok(worldBackups);
        }

        [HttpGet("{wId}/slot/{sId}/download")]
        [CheckForWorld]
        [CheckRealmOwner]
        public ActionResult<BackupDownloadResponse> GetBackup(int wId, int sId)
        {
            Backup backup = _context.Backups.Include(b => b.Slot).FirstOrDefault(b => b.Slot.World.Id == wId && b.Slot.Id == sId);

            if (backup == null)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 404,
                    ErrorMsg = "No backup found"
                };

                return NotFound(errorResponse);
            }

            BackupDownloadResponse backupDownloadResponse = new()
            {
                DownloadLink = backup.DownloadUrl,
                ResourcePackUrl = backup.ResourcePackUrl,
                ResourcePackHash = backup.ResourcePackHash,
            };

            return Ok(backupDownloadResponse);
        }

        [HttpGet("v1/{wId}/join/pc")]
        public ActionResult<Connection> Join(int wId)
        {
            var connection = _context.Connections.Include(c => c.World).FirstOrDefault(x => x.World.Id == wId);

            // Set the server's addrees to its MOTD
            connection.Address = connection.World.Motd;

            return Ok(connection);
        }

        [HttpDelete("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        public ActionResult<bool> DeleteRealm(int wId)
        {
            var world = _context.Worlds.Find(wId);

            if (world.ParentWorld == null)
            {
                var snapshotWorld = _context.Worlds.FirstOrDefault(w => w.ParentWorld.Id == wId);

                if (snapshotWorld != null)
                {
                    _context.Worlds.Remove(snapshotWorld);
                }
            }
            else
            {
                _context.Worlds.Remove(world.ParentWorld);
            }

            _context.Worlds.Remove(world);
            _context.SaveChanges();

            return Ok(true);
        }

        [HttpGet("templates/{type}")]
        public ActionResult<TemplatesResponse> GetWorldTemplates(string type, int page, int pageSize)
        {
            var totalTemplates = _context.Templates.Where(t => t.Type == type).Count();
            var templates = _context.Templates.Where(t => t.Type == type).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        TemplatesResponse templatesResponse = new()
            {
                Page = page,
                Size = pageSize,
                Total = totalTemplates,
                Templates = templates
            };

            return Ok(templatesResponse);
        }

        [HttpPut("minigames/{mId}/{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        public ActionResult<bool> SwitchToMinigame(int mId, int wId)
        {
            var world = _context.Worlds.Find(wId);
            var minigame = _context.Templates.FirstOrDefault(t => t.Type == nameof(WorldTemplateTypeEnum.MINIGAME) && t.Id == mId);

            world.Minigame = minigame;
            world.WorldType = nameof(WorldTypeEnum.MINIGAME);

            _context.SaveChanges();

            return Ok(true);
        }
    }
}
