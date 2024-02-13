using Microsoft.EntityFrameworkCore;

namespace Minecraft_Realms_Emulator.Entities
{
    [Keyless]
    public class Connection
    {
        public World World { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool PendingUpdate { get; set; }
    }
}
