using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Shared.Attributes;
using Minecraft_Realms_Emulator.Shared.Responses;

namespace Minecraft_Realms_Emulator.Modes.External
{
    [Route("modes/external/[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class ActivitiesController : ControllerBase
    {
        [HttpGet("liveplayerlist")]
        public ActionResult<LivePlayerListsResponse> GetLivePlayerList()
        {
            LivePlayerListsResponse response = new()
            {
                Lists = []
            };

            return Ok(response);
        }
    }
}
