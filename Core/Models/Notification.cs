namespace Core.Models;

public class Notification
{
    public required string NotificationUuid { get; set; }
    public bool Dismissable { get; set; }
    public bool Seen { get; set; }
    public required string Type { get; set; }
    public required RealmsText Message { get; set; }

    // visitUrl
    public string? Url { get; set; }
    public RealmsText? ButtonText { get; set; }

    // infoPopup
    public RealmsText? Title { get; set; }
    public string? Image { get; set; }
    public UrlButton? UrlButton { get; set; }
}
