namespace Minecraft_Realms_Emulator.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int WorldId { get; set; }
        public World World { get; set; } = null!;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public string SubscriptionType { get; set; } = "NORMAL";
    }
}