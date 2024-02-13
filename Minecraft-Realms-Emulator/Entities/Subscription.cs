namespace Minecraft_Realms_Emulator.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public World World { get; set; }
        public string StartDate { get; set; } = ((DateTimeOffset) DateTime.Now).ToUnixTimeMilliseconds().ToString();
        public string SubscriptionType { get; set; } = "NORMAL";
    }
}