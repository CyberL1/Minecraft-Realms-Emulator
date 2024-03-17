namespace Minecraft_Realms_Emulator.Entities
{
    public class WorldResponse : World
    {
        public string RemoteSubscriptionId { get; set; } = Guid.NewGuid().ToString();
        public int DaysLeft { get; set; } = 30;
        public bool Expired { get; set; } = false;
        public bool ExpiredTrial { get; set; } = false;
    }
}