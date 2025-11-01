namespace Minecraft_Realms_Emulator.Responses;

public class BackupUploadResponse
{
    public required string Token { set; get; } = "";
    public required string UploadEndpoint { set; get; }
    public required bool WorldClosed { set; get; }
}