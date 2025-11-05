using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers.Config;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class TrialController : ControllerBase
    {
        [HttpGet]
        public ActionResult<bool> GetTrial()
        {
            var trialMode = ConfigHelper.GetSetting(nameof(SettingsEnum.TrialMode));
            return Ok(trialMode);
        }
    }
}
