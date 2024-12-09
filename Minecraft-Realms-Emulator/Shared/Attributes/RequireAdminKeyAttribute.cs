namespace Minecraft_Realms_Emulator.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireAdminKeyAttribute : Attribute
    {
        public bool HasAdminKey(string authorization)
        {
            return authorization != null && authorization == Environment.GetEnvironmentVariable("ADMIN_KEY");
        }
    }
}
