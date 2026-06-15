namespace Core.Entities;

public class Subscription
{
    public required int Id { get; set; }
    public required string SubscriptionId { get; set; }
    public required int DaysLeft { get; set; }

    // Helper fields
    public bool Ended => DaysLeft > 0;
}