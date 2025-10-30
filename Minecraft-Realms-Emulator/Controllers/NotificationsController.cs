using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
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
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            List<NotificationResponse> notifications = [];

            foreach (var notification in _context.Notifications.ToList())
            {
                var seen = _context.SeenNotifications.Any(n => n.PlayerUUID == playerUUID && n.NotificationUUID == notification.NotificationUuid);

                if (seen)
                {
                    continue;
                }

                NotificationResponse notificationResponse = new()
                {
                    NotificationUuid = notification.NotificationUuid,
                    Dismissable = notification.Dismissable,
                    Seen = seen,
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

        [HttpPost("seen")]
        public ActionResult Seen(string[] notificationIds)
        {
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            if (notificationIds.Length == 0) {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 400,
                    ErrorMsg = "Nothing to mark as seen"
                };

                return BadRequest(errorResponse);
            }

            foreach (var notificationId in notificationIds)
            {
                SeenNotification notificationSeen = new()
                {
                    PlayerUUID = playerUUID,
                    NotificationUUID = notificationId
                };

                _context.SeenNotifications.Add(notificationSeen);
            }

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("dismiss")]
        public ActionResult Dismiss(string[] notificationIds)
        {
            return NoContent();
        }
    }
}
