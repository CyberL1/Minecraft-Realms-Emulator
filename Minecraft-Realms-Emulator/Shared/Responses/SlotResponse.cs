namespace Minecraft_Realms_Emulator.Shared.Responses
{
    public class SlotResponse
    {
        public int SlotId { get; set; }
        public SlotOptionsResponse Options { get; set; } = null!;

        public SlotResponse()
        {
            Options = new SlotOptionsResponse();
        }
    }
}
