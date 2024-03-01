using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class McoController : ControllerBase
    {
        private readonly DataContext _context;

        public McoController(DataContext context)
        {
            _context = context;
        }

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
            var newsLink = _context.Configuration.FirstOrDefault(s => s.Key == "newsLink");

            var news = new NewsResponse
            {
                NewsLink = newsLink.Value
            };

            return news;
        }
    }
}