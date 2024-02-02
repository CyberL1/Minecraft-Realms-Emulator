using Microsoft.AspNetCore.Mvc;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TrialController : ControllerBase
    {
        [HttpGet(Name = "GetTrial")]
        public bool Get() { 
            return true;
        }
    }
}
