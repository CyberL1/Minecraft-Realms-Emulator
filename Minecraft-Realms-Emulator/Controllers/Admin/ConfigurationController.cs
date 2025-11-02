using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Helpers.Config;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [RequireAdminKey]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Configuration> GetConfiguration()
        {
            var config = ConfigHelper.GetConfig();
            return Ok(config);
        }
    }
}