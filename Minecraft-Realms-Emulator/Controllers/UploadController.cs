using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Helpers;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]

    public class UploadController : ControllerBase
    {
        [HttpPost("{wId}/{sId}")]
        public async Task<ActionResult<bool>> HandleUpload(int wId, int sId)
        {
            var worldPath = Path.Combine("/tmp", $"realm-world-{wId}");

            if (Directory.Exists(worldPath)) Directory.Delete(worldPath, true);
            Directory.CreateDirectory(worldPath);

            var gzipPath = Path.Combine(worldPath, $"slot-{sId}.tar.gz");

            await using (var stream = new FileStream(gzipPath, FileMode.Create, FileAccess.Write))
            {
                await Request.Body.CopyToAsync(stream);
            }

            var container = new DockerHelper(wId);

            await container.RunCommand($"rm -rf slot-{sId}");

            await using (var gzipStream = System.IO.File.OpenRead(gzipPath))
            {
                await container.Copy(gzipStream);
            }

            await container.RunCommand($"mv world slot-{sId}");
            await container.StopServer(true);

            return Ok(true);
        }
    }
}