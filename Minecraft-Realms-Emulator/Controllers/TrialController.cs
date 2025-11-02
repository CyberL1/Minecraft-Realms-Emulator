using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers.Config;
using Minecraft_Realms_Emulator.Data;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class TrialController(DataContext context) : ControllerBase
    {
        [HttpGet]
        public ActionResult<bool> GetTrial()
        {
            var trialMode = ConfigHelper.GetSetting(nameof(SettingsEnum.TrialMode));
            return Ok(trialMode);
        }
    }
}
