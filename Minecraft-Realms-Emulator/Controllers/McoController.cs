using Microsoft.AspNetCore.Mvc;

namespace Minecraft_Realms_Emulator.Controllers.Mco
{
    [Route("[controller]")]
    [ApiController]
    public class McoController : ControllerBase
    {
        [HttpGet("available")]
        public bool GetAvailable()
        { 
            return true;
        }

        [HttpGet("client/compatible")]
        public string GetCompatible()
        {
            return Compatility.COMPATIBLE.ToString();
        }
    }
}