using System.Text.Json;

namespace Minecraft_Realms_Emulator.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string NotificationUuid { get; set; } = null!;
        public bool Dismissable { get; set; }
        public string Type { get; set; } = null!;
        public JsonDocument? Title { get; set; } = null!;
        public JsonDocument Message { get; set; } = null!;
        public string? Image { get; set; } = null!;
        public JsonDocument? UrlButton { get; set; } = null!;
        public string? Url { get; set; } = null!;
        public JsonDocument? ButtonText { get; set; } = null!;
    }
}