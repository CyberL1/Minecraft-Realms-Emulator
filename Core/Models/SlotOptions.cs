using Core.Enums;

namespace Core.Models;

public class SlotOptions
{
    public int Id { get; set; }
    public int SpawnProtection { get; set; }
    public bool ForceGameMode { get; set; }
    public SlotOptionsDifficulty Difficulty { get; set; }
    public SlotOptionsGameMode GameMode { get; set; }
    public string SlotName { get; set; } = null!;
    public required string Version { get; set; }
}
