using System.Text.Json;

namespace Minecraft_Realms_Emulator.Shared.Entities
{
    public class Backup
    {
        public int Id { get; set; }
        public Slot Slot { get; set; } = null!;
        public string BackupId { get; set; } = null!;
        public long LastModifiedDate { get; set; }
        public int Size { get; set; }
        public JsonDocument Metadata { get; set; } = null!;
        public string DownloadUrl { get; set; } = null!;
        public string? ResourcePackUrl { get; set; } = null!;
        public string? ResourcePackHash { get; set; } = null!;
    }
}
