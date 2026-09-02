using Core.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    [HttpPost("{realmId:int}/{slotId:int}")]
    public async Task<ActionResult<PendingInvitesList>> Upload()
    {
        return Ok(true);
    }
}
