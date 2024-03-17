using System.Text.Json;

namespace Minecraft_Realms_Emulator.Entities
{
    public class World
    {
        public int Id { get; set; }
        public Subscription? Subscription { get; set; }
        public string? Owner { get; set; }
        public string? OwnerUUID { get; set; }
        public string? Name { get; set; }
        public string? Motd { get; set; }
        public string State { get; set; } = "OPEN";
        public string WorldType { get; set; } = "NORMAL";
        public List<Player> Players { get; set; } = [];
        public int MaxPlayers { get; set; } = 10;
        public string? MinigameName { get; set; }
        public int? MinigameId { get; set; }
        public string? MinigameImage { get; set; }
        public int ActiveSlot { get; set; } = 1;
        public JsonDocument[] Slots { get; set; } = [];
        public bool Member { get; set; } = false;
    }
}