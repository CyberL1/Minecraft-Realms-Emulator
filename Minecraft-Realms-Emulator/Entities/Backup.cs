using System.Text.Json;

namespace Minecraft_Realms_Emulator.Entities
{
    public class Backup
    {
        public int Id { get; set; }
        public required Slot Slot { get; set; }
        public required string BackupId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int Size { get; set; }
        public required JsonDocument Metadata { get; set; }
        public required string DownloadUrl { get; set; }
        public string? ResourcePackUrl { get; set; }
        public string? ResourcePackHash { get; set; }
    }
}
