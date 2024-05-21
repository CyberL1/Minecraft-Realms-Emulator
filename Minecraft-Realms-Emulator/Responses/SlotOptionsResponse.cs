using Minecraft_Realms_Emulator.Requests;

namespace Minecraft_Realms_Emulator.Responses
{
    public class SlotOptionsResponse : SlotOptionsRequest
    {
        public string Compatibility { get; set; } = null!;
    }
}
