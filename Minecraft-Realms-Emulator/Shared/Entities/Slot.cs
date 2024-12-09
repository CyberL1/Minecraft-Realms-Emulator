using Minecraft_Realms_Emulator.Shared.Requests;

namespace Minecraft_Realms_Emulator.Shared.Entities
{
    public class Slot : SlotOptionsRequest
    {
        public int Id { get; set; }
        public World World { get; set; } = null!;
        public int SlotId { get; set; }
    }
}
