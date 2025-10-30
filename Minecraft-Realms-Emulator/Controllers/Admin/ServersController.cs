using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [RequireAdminKey]
    public class ServersController(DataContext context) : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<World>> GetWorlds()
        {
            var worlds = context.Worlds.ToList();

            return Ok(worlds);
        }

        [HttpGet("{wId}")]
        [CheckForWorld]
        public ActionResult<World> GetWorld(int wId)
        {
            var world = context.Worlds.ToList().Find(w => w.Id == wId);

            return Ok(world);
        }

        [HttpGet("{wId}/logs")]
        public async Task<ActionResult> GetLogs(int wId)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("X-Accel-Buffering", "no");

            var world = context.Worlds.ToList().Find(w => w.Id == wId);

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
        [CheckForWorld]
        public ActionResult<bool> OpenServer(int wId)
        {
            var world = context.Worlds.ToList().Find(w => w.Id == wId);
            world.State = "OPEN";
            context.SaveChanges();

            new DockerHelper(world).StartServer();

            return Ok(true);
        }

        [HttpPut("{wId}/close")]
        [CheckForWorld]
        public ActionResult<bool> CloseServer(int wId)
        {
            var world = context.Worlds.ToList().Find(w => w.Id == wId);

            world.State = "CLOSED";
            context.SaveChanges();

            new DockerHelper(world).StopServer();

            return Ok(true);
        }
    }
}