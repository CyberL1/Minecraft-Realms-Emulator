using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Shared.Attributes;
using Minecraft_Realms_Emulator.Shared.Enums;
using Minecraft_Realms_Emulator.Shared.Helpers;
using Minecraft_Realms_Emulator.Shared.Data;

namespace Minecraft_Realms_Emulator.Modes.External
{
    [Route("modes/external/[controller]")]
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
