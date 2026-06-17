using System.Text.Json.Serialization;

namespace Core.Entities;

public class SlotOptions
{
    public int Id { get; set; }
    public int SpawnProtection { get; set; }
    public bool ForceGamemode { get; set; }
    public int Difficulty { get; set; }
    public int Gamemode { get; set; }
    public string SlotName { get; set; } = null!;
    public required string Version { get; set; }
    [JsonIgnore] public int SlotId { get; set; }
    [JsonIgnore] public Slot Slot { get; set; } = null!;
}
