namespace Minecraft_Realms_Emulator.Shared.Entities
{
    public class SeenNotification
    {
        public int Id { get; set; }
        public string PlayerUUID { get; set; } = null!;
        public string NotificationUUID { get; set; } = null!;
    }
}
