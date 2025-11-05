namespace Minecraft_Realms_Emulator.Entities
{
    public class SeenNotification
    {
        public int Id { get; set; }
        public required string PlayerUUID { get; set; }
        public required string NotificationUUID { get; set; }
    }
}
