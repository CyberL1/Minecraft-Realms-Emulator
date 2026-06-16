using System.Text.Json.Serialization;

namespace Core.Entities;

public class Subscription
{
    public int Id { get; set; }
    public required string SubscriptionId { get; set; }
    public required DateTime StartDate { get; set; }
    public required string Type { get; set; }
    [JsonIgnore] public int? RealmId { get; set; }

    [JsonIgnore] public Realm Realm { get; set; } = null!;

    // Helper fields
    public int DaysLeft => (StartDate.AddDays(30) - DateTime.Today).Days;
    public bool Ended => DaysLeft < 0;
}
