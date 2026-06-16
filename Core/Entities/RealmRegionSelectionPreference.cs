using System.Text.Json.Serialization;
using Core.Enums;

namespace Core.Entities;

public class RealmRegionSelectionPreference
{
    [JsonIgnore] public int Id { get; set; }
    [JsonIgnore] public int RealmId { get; set; }
    [JsonIgnore] public Realm Realm { get; set; } = null!;
    public required RegionSelectionPreference RegionSelectionPreference { get; set; }
    public required string PreferredRegion { get; set; }
}
