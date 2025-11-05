using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Responses
{
    public class NotificationsResponse
    {
        public required List<NotificationResponse> Notifications { get; set; }
    }

    public class NotificationResponse : Notification
    {
        public bool Seen { get; set; }
    }
}
