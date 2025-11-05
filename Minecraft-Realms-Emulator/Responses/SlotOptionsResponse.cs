using Minecraft_Realms_Emulator.Requests;

namespace Minecraft_Realms_Emulator.Responses
{
    public class SlotOptionsResponse : SlotOptionsRequest
    {
        public required string Compatibility { get; set; }
        public new bool Hardcore { get; set; }
    }
}
