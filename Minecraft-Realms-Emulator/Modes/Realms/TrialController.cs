using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;

namespace Minecraft_Realms_Emulator.Modes.Realms
{
    [Route("modes/realms/[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class TrialController : ControllerBase
    {
        private readonly DataContext _context;

        public TrialController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<bool> GetTrial()
        {
            var config = new ConfigHelper(_context);
            var trialMode = config.GetSetting(nameof(SettingsEnum.TrialMode));

            return Ok(trialMode.Value);
        }
    }
}
