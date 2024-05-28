using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [RequireAdminKey]
    public class ServersController : ControllerBase
    {
        private readonly DataContext _context;

        public ServersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<World>> GetWorlds()
        {
            var worlds = _context.Worlds.ToList();

            return Ok(worlds);
        }

        [HttpGet("{wId}")]
        public ActionResult<World> GetWorld(int wId) {
            var world = _context.Worlds.ToList().Find(w => w.Id == wId);
            
            return Ok(world);
        }
    }
}