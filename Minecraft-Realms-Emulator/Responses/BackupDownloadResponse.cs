namespace Minecraft_Realms_Emulator.Responses
{
    public class BackupDownloadResponse
    {
        public string DownloadLink { get; set; } = null!;
        public string? ResourcePackUrl { get; set; }
        public string? ResourcePackHash { get; set; }
    }
}
