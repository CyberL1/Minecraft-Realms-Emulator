﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Requests;
using Minecraft_Realms_Emulator.Responses;
using Newtonsoft.Json;
using Semver;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
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
                    State = "UNINITIALIZED",
                    WorldType = "NORMAL",
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

                int versionsCompared = SemVersion.Parse(gameVersion, SemVersionStyles.Strict).ComparePrecedenceTo(SemVersion.Parse(activeSlot?.Version ?? gameVersion, SemVersionStyles.Strict));
                string isCompatible = versionsCompared == 0 ? "COMPATIBLE" : versionsCompared < 0 ? "NEEDS_DOWNGRADE" : "NEEDS_UPGRADE";

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
                string isCompatible = versionsCompared == 0 ? "COMPATIBLE" : versionsCompared < 0 ? "NEEDS_DOWNGRADE" : "NEEDS_UPGRADE";

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
        [CheckRealmOwner]
        public async Task<ActionResult<WorldResponse>> GetWorldById(int wId)
        {
            string cookie = Request.Headers.Cookie;
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var world = await _context.Worlds.Include(w => w.Players).Include(w => w.Subscription).Include(w => w.Slots).FirstOrDefaultAsync(w => w.Id == wId);

            if (world?.Subscription == null) return NotFound("World not found");

            Slot activeSlot = world.Slots.Find(s => s.SlotId == world.ActiveSlot);

            List<SlotResponse> slots = [];

            foreach (var slot in world.Slots)
            {
                int versionsCompared = SemVersion.Parse(gameVersion, SemVersionStyles.OptionalPatch).ComparePrecedenceTo(SemVersion.Parse(slot.Version, SemVersionStyles.OptionalPatch));
                string compatibility = versionsCompared == 0 ? "COMPATIBLE" : versionsCompared < 0 ? "NEEDS_DOWNGRADE" : "NEEDS_UPGRADE";

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
        [CheckRealmOwner]
        public async Task<ActionResult<World>> Initialize(int wId, WorldCreateRequest body)
        {
            string cookie = Request.Headers.Cookie;
            string gameVersion = cookie.Split(";")[2].Split("=")[1];

            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            if (world == null) return NotFound("World not found");
            if (world.State != "UNINITIALIZED") return NotFound("World already initialized");

            var subscription = new Subscription
            {
                StartDate = DateTime.UtcNow,
                SubscriptionType = "NORMAL"
            };

            world.Name = body.Name;
            world.Motd = body.Description;
            world.State = "OPEN";
            world.Subscription = subscription;

            var defaultServerAddress = _context.Configuration.FirstOrDefault(x => x.Key == "defaultServerAddress");

            var connection = new Connection
            {
                World = world,
                Address = JsonConvert.DeserializeObject(defaultServerAddress.Value)
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

        [HttpPost("{wId}/reset")]
        [CheckRealmOwner]
        public ActionResult<bool> Reset(int wId)
        {
            Console.WriteLine($"Resetting world {wId}");
            return Ok(true);
        }

        [HttpPut("{wId}/open")]
        [CheckRealmOwner]
        public async Task<ActionResult<bool>> Open(int wId)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            world.State = "OPEN";

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("{wId}/close")]
        [CheckRealmOwner]
        public async Task<ActionResult<bool>> Close(int wId)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.FirstOrDefault(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            world.State = "CLOSED";

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}")]
        [CheckRealmOwner]
        public async Task<ActionResult<bool>> UpdateWorld(int wId, WorldCreateRequest body)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            world.Name = body.Name;
            world.Motd = body.Description;

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}/slot/{sId}")]
        [CheckRealmOwner]
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
        [CheckRealmOwner]
        public ActionResult<bool> SwitchSlot(int wId, int sId)
        {
            var world = _context.Worlds.Find(wId);

            if (world == null) return NotFound("World not found");

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
        [CheckRealmOwner]
        public ActionResult<bool> DeleteRealm(int wId)
        {
            var world = _context.Worlds.Find(wId);

            if (world == null) return NotFound("World not found");

            _context.Worlds.Remove(world);
            _context.SaveChanges();

            return Ok(true);
        }
    }
}
