using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Shared.Attributes;

namespace Minecraft_Realms_Emulator.Modes.Realms.Controllers
{
    [Route("modes/realms/[controller]")]
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