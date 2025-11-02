using System.ComponentModel.DataAnnotations.Schema;
using Minecraft_Realms_Emulator.Objects;

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
        public string WorldType { get; set; } = "NORMAL";
        public List<Player> Players { get; set; } = [];
        public int MaxPlayers { get; set; } = 10;
        public Template? Minigame { get; set; }
        public Slot? ActiveSlot { get; set; }
        public List<Slot> Slots { get; set; } = [];
        public bool Member { get; set; } = false;
        public World? ParentWorld { get; set; }
        public RegionSelectionPreferenceObject RegionSelectionPreference { get; set; } = new();
    }
}