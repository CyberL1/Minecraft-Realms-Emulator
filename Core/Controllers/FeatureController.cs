using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class FeatureController : ControllerBase
{
    [HttpGet("v1")]
    public ActionResult GetFeatureFlags()
    {
        string[] featureFlags = ["realms_in_aks"];
        return Ok(featureFlags);
    }
}
