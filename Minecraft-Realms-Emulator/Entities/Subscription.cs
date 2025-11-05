namespace Minecraft_Realms_Emulator.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int WorldId { get; set; }
        public World World { get; set; } = null!; // Temp?
        public DateTime StartDate { get; set; }
        public required string SubscriptionType { get; set; }
    }
}