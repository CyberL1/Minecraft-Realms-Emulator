namespace Core.Models.Responses;

public class Subscription
{
    public required long StartDate { get; set; }
    public required int DaysLeft { get; set; }
    public required string SubscriptionType { get; set; }
}
