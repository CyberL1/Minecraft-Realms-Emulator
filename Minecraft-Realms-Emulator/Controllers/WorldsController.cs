﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
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

            var ownedWorlds = await _context.Worlds.Where(w => w.OwnerUUID == playerUUID).ToListAsync();
            var memberWorlds = await _context.Players.Where(p => p.Uuid == playerUUID && p.Accepted).Select(p => p.World).ToListAsync();

            if (ownedWorlds.ToArray().Length == 0)
            {
                var world = new World
                {
                    RemoteSubscriptionId = Guid.NewGuid().ToString(),
                    Owner = playerName,
                    OwnerUUID = playerUUID,
                    Name = null,
                    Motd = null,
                    State = State.UNINITIALIZED.ToString(),
                    Expired = false,
                    ExpiredTrial = false,
                    WorldType = WorldType.NORMAL.ToString(),
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

            List<World> allWorlds = [];

            allWorlds.AddRange(ownedWorlds);
            allWorlds.AddRange(memberWorlds);

            ServersResponse servers = new()
            {
                Servers = allWorlds
            };

            return Ok(servers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<World>> GetWorldById(int id)
        {
            var world = await _context.Worlds.Include(w => w.Players).FirstOrDefaultAsync(w => w.Id == id);

            if (world == null) return NotFound("World not found");

            return world;
        }

        [HttpPost("{id}/initialize")]
        public async Task<ActionResult<World>> Initialize(int id, WorldCreate body)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == id);

            if (world == null) return NotFound("World not found");
            if (world.State != State.UNINITIALIZED.ToString()) return NotFound("World already initialized");

            world.Name = body.Name;
            world.Motd = body.Description;
            world.State = State.OPEN.ToString();

            var subscription = new Subscription
            {
                World = world,
                StartDate = DateTime.UtcNow,
                SubscriptionType = SubscriptionType.NORMAL.ToString()
            };

            var connection = new Connection
            {
                World = world,
                Address = _context.Configuration.FirstOrDefault(x => x.Key == "defaultServerAddress").Value
            };

            _context.Worlds.Update(world);

            _context.Subscriptions.Add(subscription);
            _context.Connections.Add(connection);

            _context.SaveChanges();

            return Ok(world);
        }

        [HttpPost("{id}/reset")]
        public ActionResult<bool> Reset(int id)
        {
            Console.WriteLine($"Resetting world {id}");
            return Ok(true);
        }

        [HttpPut("{id}/open")]
        public async Task<ActionResult<bool>> Open(int id)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == id);

            if (world == null) return NotFound("World not found");

            world.State = State.OPEN.ToString();

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("{id}/close")]
        public async Task<ActionResult<bool>> Close(int id)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == id);

            if (world == null) return NotFound("World not found");

            world.State = State.CLOSED.ToString();

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<bool>> UpdateWorld(int id, WorldCreate body)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == id);

            if (world == null) return NotFound("World not found");

            world.Name = body.Name;
            world.Motd = body.Description;

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}/slot/{sId}")]
        public bool U(int wId, int sId, object o)
        {
            Console.WriteLine(o);
            return true;
        }

        [HttpGet("{Id}/backups")]
        public async Task<ActionResult<BackupList>> GetBackups(int id)
        {
            var backups = await _context.Backups.Where(b => b.World.Id == id).ToListAsync();

            BackupList worldBackups = new()
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
    }
}
