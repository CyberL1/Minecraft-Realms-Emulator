using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Helpers.Config;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [RequireAdminKey]
    public class ConfigurationController(DataContext context) : ControllerBase
    {
        [HttpGet]
        public ActionResult<Configuration> GetConfiguration()
        {
            var config = new ConfigHelper(context);
            var settings = config.GetSettings();

            return Ok(settings);
        }
    }
}