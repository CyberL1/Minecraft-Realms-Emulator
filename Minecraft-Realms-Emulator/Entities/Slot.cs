using Minecraft_Realms_Emulator.Requests;

namespace Minecraft_Realms_Emulator.Entities
{
    public class Slot : SlotOptionsRequest
    {
        public int Id { get; set; }
        public required World World { get; set; }
        public int SlotId { get; set; }
    }
}
