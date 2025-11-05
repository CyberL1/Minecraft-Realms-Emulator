namespace Minecraft_Realms_Emulator.Responses
{
    public class BackupDownloadResponse
    {
        public required string DownloadLink { get; set; }
        public string? ResourcePackUrl { get; set; }
        public string? ResourcePackHash { get; set; }
    }
}
