using Minecraft_Realms_Emulator.Enums;

namespace Minecraft_Realms_Emulator.Requests
{
    public class SlotOptionsRequest
    {
        public string SlotName { get; set; } = string.Empty;
        public required string Version { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public GamemodeEnum GameMode { get; set; }
        public bool ForceGameMode { get; set; }
        public int SpawnProtection { get; set; }
        public bool Hardcore { get; set; }
    }
}
