using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Modes.Realms.Controllers
{
    [Route("/modes/realms/[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class NotificationsController : ControllerBase
    {
        private readonly DataContext _context;

        public NotificationsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<NotificationsResponse> GetNotifications()
        {
            List<NotificationResponse> notifications = [];

            foreach (var notification in _context.Notifications)
            {
                NotificationResponse notificationResponse = new()
                {
                    NotificationUuid = notification.NotificationUuid,
                    Dismissable = notification.Dismissable,
                    Seen = false,
                    Type = notification.Type,
                    Title = notification.Title,
                    Message = notification.Message,
                    Image = notification.Image,
                    UrlButton = notification.UrlButton,
                    Url = notification.Url,
                    ButtonText = notification.ButtonText,
                };

                notifications.Add(notificationResponse);
            }

            NotificationsResponse notificationsResponse = new()
            {
                Notifications = notifications,
            };

            return Ok(notificationsResponse);
        }
    }
}
