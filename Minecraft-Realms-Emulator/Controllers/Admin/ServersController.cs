using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("{wId:int}")]
        [CheckForWorld]
        public ActionResult<World> GetWorld(int wId)
        {
            var world = context.Worlds.ToList().Find(w => w.Id == wId);

            return Ok(world);
        }

        [HttpGet("{wId:int}/logs")]
        public async Task<ActionResult> GetLogs(int wId)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("X-Accel-Buffering", "no");

            var world = context.Worlds.ToList().Find(w => w.Id == wId);

            if (world == null) return BadRequest("World not found");

            await new DockerHelper(world.Id).GetServerLogsStreamAsync(async log =>
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

        [HttpPut("{wId:int}/open")]
        [CheckForWorld]
        public async Task<ActionResult<bool>> OpenServer(int wId)
        {
            var world = context.Worlds.Include(world => world.ActiveSlot).ToList().Find(w => w.Id == wId);
            if (world is { ActiveSlot: not null })
            {
                await new DockerHelper(world.Id).StartServer(world.ActiveSlot.Id);
            }

            return Ok(true);
        }

        [HttpPut("{wId:int}/close")]
        [CheckForWorld]
        public async Task<ActionResult<bool>> CloseServer(int wId)
        {
            await new DockerHelper(wId).StopServer();
            return Ok(true);
        }
    }
}