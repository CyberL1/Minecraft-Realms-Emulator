namespace Minecraft_Realms_Emulator.Responses
{
    public class MinecraftPlayerInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public required object Result { get; set; }
    }
}
