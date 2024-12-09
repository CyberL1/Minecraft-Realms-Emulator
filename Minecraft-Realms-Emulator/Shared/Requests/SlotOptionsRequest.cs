namespace Minecraft_Realms_Emulator.Shared.Requests
{
    public class SlotOptionsRequest
    {
        public string SlotName { get; set; } = string.Empty;
        public string Version { get; set; } = null!;
        public int GameMode { get; set; }
        public int Difficulty { get; set; }
        public int SpawnProtection { get; set; }
        public bool ForceGameMode { get; set; }
        public bool Pvp { get; set; }
        public bool SpawnAnimals { get; set; }
        public bool SpawnMonsters { get; set; }
        public bool SpawnNPCs { get; set; }
        public bool CommandBlocks { get; set; }
    }
}
