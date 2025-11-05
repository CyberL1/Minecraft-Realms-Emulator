using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckForWorldAttribute : Attribute
    {
        public bool WorldExists(World? world)
        {
            return world != null;
        }
    }
}
