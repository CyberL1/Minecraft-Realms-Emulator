namespace Core.Entities;

public class Subscription
{
    public int Id { get; set; }
    public required string SubscriptionId { get; set; }
    public required int DaysLeft { get; set; }
    public int? RealmId { get; set; }
    public Realm Realm { get; set; } = null!;

    // Helper fields
    public bool Ended => DaysLeft > 0;
}
