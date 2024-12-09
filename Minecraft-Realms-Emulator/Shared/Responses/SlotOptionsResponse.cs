using Minecraft_Realms_Emulator.Shared.Requests;

namespace Minecraft_Realms_Emulator.Shared.Responses
{
    public class SlotOptionsResponse : SlotOptionsRequest
    {
        public string Compatibility { get; set; } = null!;
        public bool Hardcore { get; set; }
    }
}
