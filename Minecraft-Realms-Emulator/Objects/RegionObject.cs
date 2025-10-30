using Minecraft_Realms_Emulator.Enums;

namespace Minecraft_Realms_Emulator.Objects;

public class RegionObject
{
    public string RegionName { get; set; } = string.Empty;
    public RegionServiceQualityEnum ServiceQuality { get; set; }
}