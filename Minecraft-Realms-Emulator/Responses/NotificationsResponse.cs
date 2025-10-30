using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Responses
{
    public class NotificationsResponse
    {
        public List<NotificationResponse> Notifications { get; set; } = null!;
    }

    public class NotificationResponse : Notification
    {
        public bool Seen { get; set; }
    }
}
