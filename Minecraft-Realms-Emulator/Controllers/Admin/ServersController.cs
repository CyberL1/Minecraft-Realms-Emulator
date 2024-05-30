using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Modes.Realms.Helpers;

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

        [HttpGet("{wId}/logs")]
        public async Task<ActionResult> GetLogs(int wId)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("X-Accel-Buffering", "no");

            var world = _context.Worlds.ToList().Find(w => w.Id == wId);

            if (world == null) return BadRequest("World not found");

            await new DockerHelper(world).GetServerLogsStreamAsync(async log =>
            {
                if (!HttpContext.Response.Body.CanWrite)
                {
                    return;
                }

                await HttpContext.Response.WriteAsync($"data: {log}\n\n");
                await HttpContext.Response.Body.FlushAsync();
            });

            return new EmptyResult();
        }

        [HttpPut("{wId}/open")]
        public ActionResult<bool> OpenServer(int wId)
        {
            var world = _context.Worlds.ToList().Find(w => w.Id == wId);

            if (world == null) return BadRequest("World not found");

            world.State = "OPEN";
            _context.SaveChanges();

            new DockerHelper(world).StartServer();

            return Ok(true);
        }

        [HttpPut("{wId}/close")]
        public ActionResult<bool> CloseServer(int wId)
        {
            var world = _context.Worlds.ToList().Find(w => w.Id == wId);

            if (world == null) return BadRequest("World not found");

            world.State = "CLOSED";
            _context.SaveChanges();

            new DockerHelper(world).StopServer();

            return Ok(true);
        }
    }
}