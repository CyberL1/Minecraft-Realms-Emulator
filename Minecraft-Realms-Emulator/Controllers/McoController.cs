using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Responses;

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

        [HttpGet("v1/news")]
        public NewsResponse GetNews()
        {
            var news = new NewsResponse
            {
                NewsLink = "https://github.com/CyberL1/Minecraft-Realms-Emulator"
            };

            return news;
        }
    }
}