using Microsoft.AspNetCore.Mvc;

namespace Minecraft_Realms_Emulator.Controllers.Mco
{
    [Route("mco/[controller]")]
    [ApiController]
    public class AvailableController : ControllerBase
    {
        [HttpGet(Name = "GetAvailable")]
        public bool Get()
        { 
            return true;
        }
    }
}
