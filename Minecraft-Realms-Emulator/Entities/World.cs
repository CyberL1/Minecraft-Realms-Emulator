namespace Minecraft_Realms_Emulator.Entities
{
    public class World
    {
        public int Id { get; set; }
        public string RemoteSubscriptionId { get; set; } = Guid.NewGuid().ToString();
        public string? Owner { get; set; }
        public string? OwnerUUID { get; set; }
        public string? Name { get; set; }
        public string? Motd { get; set; }
        public string State { get; set; } = "OPEN";
        public int DaysLeft { get; set; } = 30;
        public bool Expired { get; set; } = false;
        public bool ExpiredTrial { get; set; } = false;
        public string WorldType { get; set; } = "NORMAL";
        public string[] Players { get; set; } = [];
        public int MaxPlayers { get; set; } = 10;
        public string? MinigameName { get; set; }
        public int? MinigameId { get; set; }
        public string? MinigameImage { get; set; }
        public int ActiveSlot { get; set; } = 1;
        public string[] Slots { get; set; } = [];
        public bool Member { get; set; } = false;
    }
}