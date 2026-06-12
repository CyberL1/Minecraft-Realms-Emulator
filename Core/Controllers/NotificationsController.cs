using Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<NotificationList> GetNotifications()
    {
        var notifications = new NotificationList { Notifications = [] };

        return Ok(notifications);
    }
}
