using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;

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

        [HttpGet(Name = "GetTrial")]
        public bool Get() {
            return bool.Parse(_context.Configuration.FirstOrDefault(x => x.Key == "trialMode").Value);
        }
    }
}
