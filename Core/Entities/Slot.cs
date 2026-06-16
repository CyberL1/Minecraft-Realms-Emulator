using System.Text.Json.Serialization;

namespace Core.Entities;

public class Slot
{
    public int Id { get; set; }
    public required int SlotId { get; set; }
    public SlotOptions Options { get; set; } = null!;
    public WorldSettings Settings { get; set; } = null!;
    [JsonIgnore] public int? RealmId { get; set; }
    [JsonIgnore] public Realm Realm { get; set; } = null!;
}
