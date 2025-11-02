using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Responses
{
    public class WorldResponse : World
    {
        public string RemoteSubscriptionId { get; set; } = new Guid().ToString();
        public bool IsHardcore { get; set; } = false;
        public int GameMode { get; set; }
        public int DaysLeft { get; set; } = 30;
        public bool Expired { get; set; } = false;
        public bool ExpiredTrial { get; set; } = false;
        public bool GracePeriod { get; set; } = false;
        public string Compatibility { get; set; } = null!;
        public List<SlotResponse> Slots { get; set; } = null!;
        public string ActiveVersion { get; set; } = null!;
        public int? ParentWorldId { get; set; }
        public string? ParentWorldName { get; set; }
        public int? MinigameId { get; set; }
        public string? MinigameName { get; set; }
        public string? MinigameImage { get; set; }
        public required string State { get; set; }
    }
}