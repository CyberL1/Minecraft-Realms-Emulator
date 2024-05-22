using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Helpers;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
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
            var trialMode = config.GetSetting("trialMode");

            return Ok(trialMode.Value);
        }
    }
}
