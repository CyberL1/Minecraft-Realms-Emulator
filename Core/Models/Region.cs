using Core.Enums;

namespace Core.Models;

public class Region
{
    public required string RegionName { get; set; }
    public required ServiceQuality ServiceQuality { get; set; }
}
