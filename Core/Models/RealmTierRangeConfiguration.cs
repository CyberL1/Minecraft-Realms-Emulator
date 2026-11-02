namespace Core.Models;

public class RealmTierRangeConfiguration
{
    public int Min { get; set; }
    public int Max { get; set; }
    public int DefaultValue { get; set; }
    public int? Current { get; set; }
}