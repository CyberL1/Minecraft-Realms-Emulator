using System.IO.Compression;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;

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
            var tarPath = Path.Combine(worldPath, $"slot-{sId}.tar");

            await using (var stream = new FileStream(gzipPath, FileMode.Create, FileAccess.Write))
            {
                await Request.Body.CopyToAsync(stream);
            }

            await using (var originalFileStream = System.IO.File.OpenRead(gzipPath))
            await using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
            await using (var tarFileStream = System.IO.File.Create(tarPath))
            {
                await decompressionStream.CopyToAsync(tarFileStream);
            }

            await using (var tarStream = System.IO.File.OpenRead(tarPath))
            using (var tarArchive = TarArchive.CreateInputTarArchive(tarStream))
            {
                tarArchive.ExtractContents(worldPath);
            }

            // TODO: actually upload the world to server

            return Ok(true);
        }
    }
}