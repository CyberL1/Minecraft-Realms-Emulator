using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[Route("[controller]")]
[ApiController]
public class TrialController : Controller
{
    [HttpGet]
    public ActionResult GetTrial()
    {
        return Ok(true);
    }
}
