using Minecraft_Realms_Emulator.Enums;

namespace Minecraft_Realms_Emulator.Objects;

public class RegionSelectionPreferenceObject
{
    public RegionSelectionPreferenceEnum RegionSelectionPreference { get; set; }
    public string? PreferredRegion { get; set; }
}