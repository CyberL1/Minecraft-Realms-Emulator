using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("worlds/v1/{worldId}/join/pc")]
    [ApiController]
    public class JoinController : ControllerBase
    {
        private readonly DataContext _context;

        public JoinController(DataContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public ActionResult<Connection> Join(int worldId)
        {
            var connection = _context.Connections.FirstOrDefault(x => x.World.Id == worldId);

            return Ok(connection);
        }
    }
}
