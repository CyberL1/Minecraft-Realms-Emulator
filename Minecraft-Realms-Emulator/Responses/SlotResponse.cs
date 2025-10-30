using Minecraft_Realms_Emulator.Objects;

namespace Minecraft_Realms_Emulator.Responses
{
    public class SlotResponse
    {
        public int SlotId { get; set; }
        public List<SlotSettingObject> Settings { get; set; } = [];
        public string Options { get; set; } = null!;
    }
}
