namespace Minecraft_Realms_Emulator.Responses
{
    public class SlotResponse
    {
        public int SlotId { get; set; }
        public SlotSettingsResponse Settings { get; set; } = null!;
        public string Options { get; set; } = null!;

        public SlotResponse()
        {
            Settings = new SlotSettingsResponse();
        }
    }
}
