namespace Minecraft_Realms_Emulator.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public string RemoteId { get; set; } = Guid.NewGuid().ToString();
        public string StartDate { get; set; } = ((DateTimeOffset) DateTime.Now).ToUnixTimeMilliseconds().ToString();
        public int DaysLeft { get; set; } = 30;
        public string SubscriptionType { get; set; } = "NORMAL";
    }
}