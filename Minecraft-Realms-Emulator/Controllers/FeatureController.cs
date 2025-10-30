using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;

namespace Minecraft_Realms_Emulator.Controllers;

[Route("[controller]")]
[ApiController]
[RequireMinecraftCookie]
public class FeatureController : ControllerBase
{ 
    [HttpGet("v1")]
    public ActionResult<List<string>> GetFeatureFlags() 
    { 
        List<string> flags = ["realms_in_aks"];
        return Ok(flags);
    }
}