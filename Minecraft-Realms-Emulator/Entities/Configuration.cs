using Microsoft.EntityFrameworkCore;

namespace Minecraft_Realms_Emulator.Entities
{
    [PrimaryKey(nameof(Key))]
    public class Configuration
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
