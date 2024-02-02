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
