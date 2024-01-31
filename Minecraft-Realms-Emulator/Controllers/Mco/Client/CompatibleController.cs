using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Minecraft_Realms_Emulator.Controllers.Mco.Client
{
    [Route("/mco/client/[controller]")]
    [ApiController]
    public class CompatibleController : ControllerBase
    {
        [HttpGet(Name = "GetCompatible")]
        public string Get()
        {
            return Compatility.COMPATIBLE.ToString();
        }
    }
}
