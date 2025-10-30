using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class RegionsController : ControllerBase
    {
        [HttpGet("ping/stat")]
        public ActionResult<string> GetRegionsStat(int wId)
        {
            return Ok(new List<string>());
        }

        [HttpGet("preferredRegions")]
        public ActionResult<string> GetPreferredRegions(int wId)
        {
            return Ok(new List<string>());
        }
    }
}