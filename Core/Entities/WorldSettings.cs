using System.Text.Json.Serialization;

namespace Core.Entities;

public class WorldSettings
{
    public int Id { get; set; }
    public bool Hardcore { get; set; }
    [JsonIgnore] public int SlotId { get; set; }
    [JsonIgnore] public Slot Slot { get; set; } = null!;
}
