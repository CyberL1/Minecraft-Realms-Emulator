namespace Minecraft_Realms_Emulator.Responses;

public class BackupResponse
{
    public int SlotId { get; set; }
    public long Size { get; set; }
    public object? Metadata { get; set; }
    public string DownloadUrl { get; set; } = "";
    public string? ResourcePackHash { get; set; }
    public string? ResourcePackUrl { get; set; }
    public long LastModifiedDate { get; set; }
}