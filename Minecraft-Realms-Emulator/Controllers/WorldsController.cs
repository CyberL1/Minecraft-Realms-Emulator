using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

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
        public async Task<ActionResult<ServersArray>> GetWorlds()
        {
            var worlds = await _context.Worlds.ToListAsync();

            string cookie = Request.Headers.Cookie;
            
            string playerUUID = cookie.Split(";")[0].Split(":")[2];
            string playerName = cookie.Split(";")[1].Split("=")[1];

            var hasWorld = worlds.Find(p => p.OwnerUUID == playerUUID);

            if (hasWorld == null)
            {
                var world = new World
                {
                    RemoteSubscriptionId = Guid.NewGuid().ToString(),
                    Owner = playerName,
                    OwnerUUID = playerUUID,
                    Name = null,
                    Motd = null,
                    State = State.UNINITIALIZED.ToString(),
                    DaysLeft = 30,
                    Expired = false,
                    ExpiredTrial = false,
                    WorldType = WorldType.NORMAL.ToString(),
                    Players = [],
                    MaxPlayers = 10,
                    MinigameId = null,
                    MinigameName = null,
                    MinigameImage = null,
                    Slots = [],
                    ActiveSlot = 1,
                    Member = false
                };

                worlds.Add(world);

                _context.Worlds.Add(world);
                _context.SaveChanges();
            }

            ServersArray servers = new()
            {
                Servers = worlds
            };

            return Ok(servers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<World>> GetWorldById(int id)
        {
            var world = await _context.Worlds.FindAsync(id);

            if (world == null) return NotFound("World not found");

            return world;
        }
    }
}
