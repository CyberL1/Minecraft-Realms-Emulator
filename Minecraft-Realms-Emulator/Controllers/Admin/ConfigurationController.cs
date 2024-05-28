using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Helpers;

namespace Minecraft_Realms_Emulator.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [RequireAdminKey]
    public class ConfigurationController : ControllerBase
    {
        private readonly DataContext _context;

        public ConfigurationController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<Configuration> GetConfiguration()
        {
            var config = new ConfigHelper(_context);
            var settings = config.GetSettings();

            return Ok(settings);
        }
    }
}