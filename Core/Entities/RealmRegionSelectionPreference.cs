using Core.Enums;

namespace Core.Entities;

public class RealmRegionSelectionPreference
{
    public int Id { get; set; }
    public int RealmId { get; set; }
    public Realm Realm { get; set; } = null!;
    public required RegionSelectionPreference RegionSelectionPreference { get; set; }
    public required string PreferredRegion { get; set; }
}
