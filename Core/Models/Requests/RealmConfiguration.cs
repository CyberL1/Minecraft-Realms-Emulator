namespace Core.Models.Requests;

public class RealmConfiguration
{
    public required SlotOptions Options { get; set; }

    public required List<RealmsSetting> Settings { get; set; }
    public RealmRegionSelectionPreference? RegionSelectionPreference { get; set; }
    public RealmDescription? Description { get; set; }
}
