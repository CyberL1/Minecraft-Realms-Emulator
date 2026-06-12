using Core.Models;

namespace Core.Responses;

public class NotificationList
{
    public required List<Notification> Notifications { get; set; }
}
