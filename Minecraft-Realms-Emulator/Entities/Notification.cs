using System.Text.Json;

namespace Minecraft_Realms_Emulator.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public required string NotificationUuid { get; set; }
        public bool Dismissable { get; set; }
        public required string Type { get; set; }
        public JsonDocument? Title { get; set; }
        public required JsonDocument Message { get; set; }
        public string? Image { get; set; }
        public required JsonDocument? UrlButton { get; set; }
        public string? Url { get; set; }
        public JsonDocument? ButtonText { get; set; }
    }
}