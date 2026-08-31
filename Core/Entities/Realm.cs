using System.Text.Json.Serialization;

namespace Core.Entities;

public class Realm
{
    public int Id { get; set; }
    [JsonIgnore] public Subscription Subscription { get; set; } = null!;
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string State { get; set; }
    public required List<Player> Players { get; set; }
    public required List<Slot> Slots { get; set; }
    public required string WorldType { get; set; }
    [JsonIgnore] public int ActiveSlotId { get; set; }
    [JsonIgnore] public Slot ActiveSlot { get; set; } = null!;
    [JsonIgnore] public Realm? ParentWorld { get; set; }
    [JsonIgnore] public RealmRegionSelectionPreference RegionSelectionPreference { get; set; }
    public required RealmConnection Connection { get; set; } = null!;

    // Helper fields
    public Player Owner => Players.First(player => player.Owner);
}
