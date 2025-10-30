using Minecraft_Realms_Emulator.Objects;

namespace Minecraft_Realms_Emulator.Requests;

public class UpdateWorldConfigurationRequest
{
    public object Options { get; set; } = new();
    public List<SlotSettingObject> Settings { get; set; } = [];
    public RegionSelectionPreferenceObject RegionSelectionPreference { get; set; } = new();
    public WorldCreateRequest Description  { get; set; } = new();
}