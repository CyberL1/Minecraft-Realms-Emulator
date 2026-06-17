using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Entities;

public class SlotOptions
{
    public int Id { get; set; }
    public int SpawnProtection { get; set; }
    public bool ForceGameMode { get; set; }
    public SlotOptionsDifficulty Difficulty { get; set; }
    public SlotOptionsGameMode GameMode { get; set; }
    public string SlotName { get; set; } = null!;
    public required string Version { get; set; }
    [JsonIgnore] public int SlotId { get; set; }
    [JsonIgnore] public Slot Slot { get; set; } = null!;
}