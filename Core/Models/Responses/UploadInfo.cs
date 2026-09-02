namespace Core.Models.Responses;

public class UploadInfo
{
    public required string UploadEndpoint { get; set; }
    public required int Port { get; set; }
    public required bool WorldClosed { get; set; }
    public required string Token { get; set; }
}
