using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Helpers.Config;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Requests;
using Minecraft_Realms_Emulator.Responses;
using Minecraft_Realms_Emulator.Objects;
using Newtonsoft.Json;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class WorldsController(DataContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ServersResponse>> GetWorlds()
        {
            string cookie = Request.Headers.Cookie;

            string playerUUID = cookie.Split(";")[0].Split(":")[2];
            string playerName = cookie.Split(";")[1].Split("=")[1];
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var ownedWorlds = await context.Worlds.Where(w => w.OwnerUUID == playerUUID).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.ActiveSlot).Include(w => w.Minigame).ToListAsync();
            var memberWorlds = await context.Players.Where(p => p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Include(p => p.World.Slots).Include(p => p.World.ActiveSlot).Include(p => p.World.Minigame).Select(p => p.World).ToListAsync();

            List<WorldResponse> allWorlds = [];

            if (ownedWorlds.ToArray().Length == 0 && ConfigHelper.GetSetting(nameof(SettingsEnum.AutomaticRealmsCreation)))
            {
                var world = new World
                {
                    Owner = playerName,
                    OwnerUUID = playerUUID,
                    Name = null,
                    Motd = null,
                    WorldType = nameof(WorldTypeEnum.NORMAL),
                    MaxPlayers = 10,
                    Minigame = null,
                    ActiveSlot = null,
                    Member = false
                };

                ownedWorlds.Add(world);
                context.Worlds.Add(world);

                context.SaveChanges();
            }

            foreach (var world in ownedWorlds)
            {
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot?.Version ?? gameVersion));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = world.ActiveSlot?.GameMode ?? 0,
                    IsHardcore = world.ActiveSlot?.Difficulty == 3,
                    State = await new WorldHelper(context, world.Id).GetState(),
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot?.SlotId ?? 1,
                    Member = world.Member,
                    Players = world.Players,
                    ActiveVersion = world.ActiveSlot?.Version ?? gameVersion,
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
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot?.Version ?? gameVersion));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = world.ActiveSlot.GameMode,
                    IsHardcore = world.ActiveSlot.Difficulty == 3,
                    State = await new WorldHelper(context, world.Id).GetState(),
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot.SlotId,
                    Member = world.Member,
                    Players = world.Players,
                    DaysLeft = 0,
                    Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                    ExpiredTrial = false,
                    ActiveVersion = world.ActiveSlot.Version,
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

            var ownedWorlds = await context.Worlds.Where(w => w.OwnerUUID == playerUUID || w.ParentWorld.OwnerUUID == playerUUID).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.ActiveSlot).Include(w => w.Minigame).Include(w => w.ParentWorld).ToListAsync();
            var memberWorlds = await context.Players.Where(p => p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Include(p => p.World.Slots).Include(p => p.World.ActiveSlot).Include(p => p.World.ParentWorld).Include(p => p.World.Minigame).Select(p => p.World).ToListAsync();

            List<WorldResponse> allWorlds = [];

            foreach (var world in ownedWorlds)
            {
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot?.Version ?? gameVersion));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = world.ActiveSlot?.GameMode ?? 0,
                    IsHardcore = world.ActiveSlot?.Difficulty == 3,
                    State = await new WorldHelper(context, world.Id).GetState(),
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot.SlotId,
                    Member = world.Member,
                    Players = world.Players,
                    ActiveVersion = world.ActiveSlot?.Version ?? gameVersion,
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
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot?.Version));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = world.ActiveSlot.GameMode,
                    IsHardcore = world.ActiveSlot.Difficulty == 3,
                    State = await new WorldHelper(context, world.Id).GetState(),
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot.SlotId,
                    Member = world.Member,
                    Players = world.Players,
                    DaysLeft = 0,
                    Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                    ExpiredTrial = false,
                    ActiveVersion = world.ActiveSlot.Version,
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

            var ownedWorlds = await context.Worlds.Where(w => w.ParentWorld != null && w.ParentWorld.OwnerUUID == playerUUID).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.ActiveSlot).Include(w => w.ParentWorld).Include(w => w.Minigame).ToListAsync();
            var memberWorlds = await context.Players.Where(p => p.World.ParentWorld != null && p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Include(p => p.World.Slots).Include(p => p.World.ActiveSlot).Include(p => p.World.ParentWorld).Include(p => p.World.Minigame).Select(p => p.World).ToListAsync();

            List<WorldResponse> allWorlds = [];

            if (ownedWorlds.ToArray().Length == 0 && ConfigHelper.GetSetting(nameof(SettingsEnum.AutomaticRealmsCreation)))
            {
                var parentWorld = context.Worlds.FirstOrDefault(w => w.OwnerUUID == playerUUID && w.ParentWorld == null);

                if (parentWorld != null && parentWorld.Name != null)
                {
                    var world = new World
                    {
                        Name = parentWorld.Name,
                        Motd = null,
                        WorldType = nameof(WorldTypeEnum.NORMAL),
                        MaxPlayers = 10,
                        Minigame = null,
                        ActiveSlot = null,
                        Member = false,
                        ParentWorld = parentWorld,
                    };

                    ownedWorlds.Add(world);
                    context.Worlds.Add(world);

                    context.SaveChanges();
                }
            }

            foreach (var world in ownedWorlds)
            {
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot?.Version ?? gameVersion));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = world.ActiveSlot?.GameMode ?? 0,
                    IsHardcore = world.ActiveSlot?.Difficulty == 3,
                    State = await new WorldHelper(context, world.Id).GetState(),
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot.SlotId,
                    Member = world.Member,
                    Players = world.Players,
                    ActiveVersion = world.ActiveSlot?.Version ?? gameVersion,
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
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot.Version));
                string isCompatible = versionsCompared == 0 ? nameof(CompatibilityEnum.COMPATIBLE) : versionsCompared < 0 ? nameof(CompatibilityEnum.NEEDS_DOWNGRADE) : nameof(CompatibilityEnum.NEEDS_UPGRADE);

                WorldResponse response = new()
                {
                    Id = world.Id,
                    Owner = world.Owner,
                    OwnerUUID = world.OwnerUUID,
                    Name = world.Name,
                    Motd = world.Motd,
                    GameMode = world.ActiveSlot.GameMode,
                    IsHardcore = world.ActiveSlot.Difficulty == 3,
                    State = await new WorldHelper(context, world.Id).GetState(),
                    WorldType = world.WorldType,
                    MaxPlayers = world.MaxPlayers,
                    ActiveSlot = world.ActiveSlot.SlotId,
                    Member = world.Member,
                    Players = world.Players,
                    DaysLeft = 0,
                    Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                    ExpiredTrial = false,
                    ActiveVersion = world.ActiveSlot.Version,
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

            var world = await context.Worlds.Include(w => w.Players).Include(w => w.Subscription).Include(w => w.Slots).Include(w => w.ActiveSlot).Include(w => w.ParentWorld.Subscription).FirstOrDefaultAsync(w => w.Id == wId);

            if (world.Name == null)
            {
                ErrorResponse error = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Initialize the world first"
                };

                return StatusCode(400, error);
            }

            List<SlotResponse> slots = [];

            foreach (var slot in world.Slots)
            {
                int versionsCompared = new MinecraftVersionParser.MinecraftVersion(gameVersion).CompareTo(new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot.Version));
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
                    }),
                    Settings = [
                    new SlotSettingObject
                    {
                        Name = "hardcore",
                        Value = slot.Difficulty == 3
                    }]
                });
            }

            var activeSlotOptions = JsonConvert.DeserializeObject<SlotOptionsResponse>(slots.Find(s => s.SlotId == world.ActiveSlot.SlotId).Options);

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
                GameMode = world.ActiveSlot.GameMode,
                IsHardcore = world.ActiveSlot.Difficulty == 3,
                State = await new WorldHelper(context, world.Id).GetState(),
                WorldType = world.WorldType,
                MaxPlayers = world.MaxPlayers,
                ActiveSlot = world.ActiveSlot.SlotId,
                Slots = slots,
                Member = world.Member,
                Players = world.Players,
                DaysLeft = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days,
                Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                ExpiredTrial = false,
                ActiveVersion = activeSlotOptions.Version,
                Compatibility = activeSlotOptions.Compatibility,
                RegionSelectionPreference = world.RegionSelectionPreference
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

            var worlds = await context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);
            if (world.Name != null)
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
            world.Subscription = subscription;

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


            context.Subscriptions.Add(subscription);
            context.Slots.Add(slot);

            world.ActiveSlot = slot;
            context.Worlds.Update(world);

            context.SaveChanges();
            await new DockerHelper(world.Id).CreateVolume();

            return Ok(world);
        }

        [HttpPost("{wId}/createPrereleaseRealm")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<World>> CreatePrereleaseRealms(int wId)
        {
            string cookie = Request.Headers.Cookie;
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var worlds = await context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            if (world.ParentWorld.Name == null)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 401,
                    ErrorMsg = "You must initialize release world first",
                };

                return StatusCode(401, errorResponse);
            }

            if (world.Name != null)
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
        [CheckActiveSubscription]
        public ActionResult<bool> Reset(int wId)
        {
            Console.WriteLine($"Resetting world {wId}");

            var world = context.Worlds.Find(wId);
            var server = new DockerHelper(world.Id);

            server.RunCommand($"rm -rf slot-{world.ActiveSlot}");
            server.StopServer();

            return Ok(true);
        }

        [HttpPut("{wId}/open")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<bool>> Open(int wId)
        {
            var world = await context.Worlds.Include(w => w.ActiveSlot).FirstAsync(w => w.Id == wId);

            var server = new DockerHelper(world.Id);
            await server.StartServer(world.ActiveSlot.SlotId);

            var defaultServerAddress = ConfigHelper.GetSetting(nameof(SettingsEnum.DefaultServerAddress));

            var serverPort = await server.GetServerPort();
            var serverAddress = $"{defaultServerAddress}:{serverPort}";

            var query = new MinecraftServerQuery().Query(serverAddress);

            while (query == null)
            {
                await Task.Delay(1000);
                query = new MinecraftServerQuery().Query(serverAddress);
            }

            return Ok(true);
        }

        [HttpPut("{wId}/close")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<bool>> Close(int wId)
        {
            var world = await context.Worlds.FindAsync(wId);

            var server = new DockerHelper(world.Id);
            await server.StopServer();

            var defaultServerAddress = ConfigHelper.GetSetting(nameof(SettingsEnum.DefaultServerAddress));

            var serverPort = await server.GetServerPort();
            var serverAddress = $"{defaultServerAddress}:{serverPort}";

            var query = new MinecraftServerQuery().Query(serverAddress);

            while (query != null)
            {
                await Task.Delay(1000);
                query = new MinecraftServerQuery().Query(serverAddress);
            }

            return Ok(true);
        }

        [HttpPost("{wId}/configuration")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<(bool, ErrorResponse)>> UpdateWorld(int wId, UpdateWorldConfigurationRequest body)
        {
            if (body.Description.Name.Trim() == "")
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "World name cannot be empty"
                };

                return BadRequest(errorResponse);
            }
            
            if (body.Description.Name.Length > 32)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "World name cannot exceed 32 characters"
                };

                return BadRequest(errorResponse);
            }

            if (body.Description.Description?.Length > 32)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "World description cannot exceed 32 characters"
                };
            
                return BadRequest(errorResponse);
            }

            var world = await context.Worlds.FindAsync(wId);

            world.Name = body.Description.Name.Trim();
            world.Motd = body.Description.Description.Trim();
            world.RegionSelectionPreference = body.RegionSelectionPreference;

            await context.SaveChangesAsync();
            return Ok(true);
        }

        [HttpPost("{wId}/slot/{sId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<(bool, ErrorResponse)>> UpdateSlot(int wId, int sId, SlotOptionsRequest body)
        {
            string cookie = Request.Headers.Cookie;
            var gameVersion = cookie.Split(";")[2].Split("=")[1];

            if (body.SlotName.Length > 32)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Slot name cannot exceed 32 characters"
                };

                return BadRequest(errorResponse);
            }

            if (body.SpawnProtection < 0 || body.SpawnProtection > 16)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Spawn protection can only be between 0 and 16"
                };

                return BadRequest(errorResponse);
            }

            if (!new List<int> { 0, 1, 2 }.Contains(body.GameMode))
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Gamemode can only be one of 0, 1, 2"
                };

                return BadRequest(errorResponse);
            }

            if (!new List<int> { 0, 1, 2, 3 }.Contains(body.Difficulty))
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Difficulty can only be one of 0, 1, 2, 3"
                };

                return BadRequest(errorResponse);
            }

            var slots = await context.Slots.Where(s => s.World.Id == wId).ToListAsync();
            var slot = slots.Find(s => s.SlotId == sId) ?? new Slot
            {
                SlotId = sId,
                World = context.Worlds.First(w => w.Id == wId),
                Version = gameVersion
            };

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

            var slotExists = context.Slots.Any(s => s.World.Id == wId && s.SlotId == sId);
            if (!slotExists)
            {
                context.Slots.Add(slot);
            }
            
            context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("{wId}/slot/{sId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public ActionResult<bool> SwitchSlot(int wId, int sId)
        {
            var world = context.Worlds.Include(w => w.Minigame).Include(w => w.ActiveSlot).FirstOrDefault(w => w.Id == wId);
            var newSlot = context.Slots.FirstOrDefault(s => s.World.Id == wId && s.SlotId == sId);

            if (newSlot == null)
            {
                string cookie = Request.Headers.Cookie;
                string gameVersion = cookie.Split(";")[2].Split("=")[1];

                var slot = new Slot
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
                };

                context.Slots.Add(slot);
                context.SaveChanges();

                newSlot = slot;
            }

            var server = new DockerHelper(world.Id);
            server.StopServer();

            world.ActiveSlot = newSlot;
            world.Minigame = null;
            world.WorldType = nameof(WorldTypeEnum.NORMAL);

            context.SaveChanges();

            return Ok(true);
        }

        [HttpGet("{wId}/backups")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<BackupsResponse>> GetBackups(int wId)
        {
            var backups = await context.Backups.Where(b => b.Slot.World.Id == wId).ToListAsync();

            BackupsResponse worldBackups = new()
            {
                Backups = backups
            };

            return Ok(worldBackups);
        }

        [HttpPut("{wId}/backups/upload")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public ActionResult<BackupUploadResponse> UploadBackup(int wId)
        {
            var response = new BackupUploadResponse
            {
                Token = Guid.NewGuid().ToString(),
                UploadEndpoint = "127.0.0.1",
                WorldClosed = true
            };

            return Ok(response);
        }

        [HttpGet("{wId}/slot/{sId}/download")]
        [CheckForWorld]
        [CheckRealmOwner]
        public ActionResult<BackupDownloadResponse> GetBackup(int wId, int sId)
        {
            Backup backup = context.Backups.Include(b => b.Slot).FirstOrDefault(b => b.Slot.World.Id == wId && b.Slot.Id == sId);

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
        [CheckActiveSubscription]
        public async Task<ActionResult<ConnectionResponse>> Join(int wId)
        {
            var world = context.Worlds.Include(w => w.Slots).Include(w => w.ActiveSlot).FirstOrDefault(w => w.Id == wId);
            var helper = new DockerHelper(world.Id);

            var isRunning = await helper.IsRunning();
            if (!isRunning)
            {
                await helper.StartServer(world.ActiveSlot.SlotId);
            }

            var serverPort = await helper.GetServerPort();

            var defaultServerAddress = ConfigHelper.GetSetting(nameof(SettingsEnum.DefaultServerAddress));
            var serverAddress = $"{defaultServerAddress}:{serverPort}";
            
            var query = new MinecraftServerQuery().Query(serverAddress);

            while (query == null)
            {
                await Task.Delay(1000);
                query = new MinecraftServerQuery().Query(serverAddress);
            }

            string cookie = Request.Headers.Cookie;
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            if (new MinecraftVersionParser.MinecraftVersion(world.ActiveSlot.Version).CompareTo(new MinecraftVersionParser.MinecraftVersion(gameVersion)) < 0 && await helper.RunCommand("! test -f .no-update") == 0)
            {
                world.ActiveSlot.Version = gameVersion;
                context.SaveChanges();
            }

            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            if (world.OwnerUUID == playerUUID)
            {
                await helper.ExecuteCommand($"op {world.Owner}");
            }

            var response = new ConnectionResponse
            {
                Address = serverAddress,
                PendingUpdate = false
            };
            
            return Ok(response);
        }

        [HttpDelete("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<bool>> DeleteRealm(int wId)
        {
            var world = await context.Worlds.Include(w => w.Subscription).Include(w => w.ParentWorld)
                .FirstAsync(w => w.Id == wId);

            if (((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days > 0)
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 403,
                    ErrorMsg = "World is not expired"
                };

                return StatusCode(403, errorResponse);
            }

            if (world.ParentWorld == null)
            {
                var snapshotWorld = context.Worlds.FirstOrDefault(w => w.ParentWorld.Id == wId);

                if (snapshotWorld != null)
                {
                    new DockerHelper(snapshotWorld.Id).DeleteServer();
                    context.Worlds.Remove(snapshotWorld);
                }
            }
            else
            {
                new DockerHelper(world.ParentWorld.Id).DeleteServer();
                context.Worlds.Remove(world.ParentWorld);
            }

            new DockerHelper(world.Id).DeleteServer();

            context.Worlds.Remove(world);
            await context.SaveChangesAsync();

            return Ok(true);
        }

        [HttpGet("templates/{type}")]
        public ActionResult<TemplatesResponse> GetWorldTemplates(string type, int page, int pageSize)
        {
            var totalTemplates = context.Templates.Where(t => t.Type == type).Count();
            var templates = context.Templates.Where(t => t.Type == type).Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
            var world = context.Worlds.Find(wId);
            var minigame = context.Templates.FirstOrDefault(t => t.Type == nameof(WorldTemplateTypeEnum.MINIGAME) && t.Id == mId);

            world.Minigame = minigame;
            world.WorldType = nameof(WorldTypeEnum.MINIGAME);

            context.SaveChanges();

            return Ok(true);
        }
    }
}
