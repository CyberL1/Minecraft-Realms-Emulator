using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[Route("[controller]")]
[ApiController]
public class McoController : Controller
{
    [HttpGet("available")]
    public ActionResult<bool> GetAvailable()
    {
        return Ok(true);
    }

    [HttpGet("client/compatible")]
    public ActionResult<string> GetClientCompatible()
    {
        return Ok(nameof(CompatibleVersionResponse.COMPATIBLE));
    }

    [HttpPost("tos/agreed")]
    public ActionResult PostTosAgreed()
    {
        return Ok();
    }
}
