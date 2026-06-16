using Core.Enums;

namespace Core.Models;

public class RealmRegionSelectionPreference
{
    public required RegionSelectionPreference RegionSelectionPreference { get; set; }
    public required string PreferredRegion { get; set; }
}
