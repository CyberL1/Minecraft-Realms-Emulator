using Minecraft_Realms_Emulator.Shared.Entities;

namespace Minecraft_Realms_Emulator.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckForWorldAttribute : Attribute
    {
        public bool WorldExists(World world)
        {
            return world != null;
        }
    }
}
