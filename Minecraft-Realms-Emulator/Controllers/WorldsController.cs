using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Requests;
using Minecraft_Realms_Emulator.Responses;
using Newtonsoft.Json;

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

            var ownedWorlds = await _context.Worlds.Where(w => w.OwnerUUID == playerUUID).Include(w => w.Subscription).ToListAsync();
            var memberWorlds = await _context.Players.Where(p => p.Uuid == playerUUID && p.Accepted).Include(p => p.World.Subscription).Select(p => p.World).ToListAsync();

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
                    Players = world.Players
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
                    ExpiredTrial = false
                };

                allWorlds.Add(response);
            }

            ServersResponse servers = new()
            {
                Servers = allWorlds
            };

            return Ok(servers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorldResponse>> GetWorldById(int id)
        {
            var world = await _context.Worlds.Include(w => w.Players).Include(w => w.Subscription).FirstOrDefaultAsync(w => w.Id == id);

            if (world?.Subscription == null) return NotFound("World not found");

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
                DaysLeft = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days,
                Expired = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days < 0,
                ExpiredTrial = false
            };

            return response;
        }

        [HttpPost("{id}/initialize")]
        public async Task<ActionResult<World>> Initialize(int id, WorldCreateRequest body)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.Find(w => w.Id == id);

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

            world.State = "OPEN";

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("{id}/close")]
        public async Task<ActionResult<bool>> Close(int id)
        {
            var worlds = await _context.Worlds.ToListAsync();

            var world = worlds.FirstOrDefault(w => w.Id == id);

            if (world == null) return NotFound("World not found");

            world.State = "CLOSED";

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<bool>> UpdateWorld(int id, WorldCreateRequest body)
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
        public async Task<ActionResult<BackupsResponse>> GetBackups(int id)
        {
            var backups = await _context.Backups.Where(b => b.World.Id == id).ToListAsync();

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
    }
}
