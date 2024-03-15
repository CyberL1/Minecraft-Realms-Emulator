namespace Minecraft_Realms_Emulator.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public World World { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public string SubscriptionType { get; set; } = "NORMAL";
    }
}