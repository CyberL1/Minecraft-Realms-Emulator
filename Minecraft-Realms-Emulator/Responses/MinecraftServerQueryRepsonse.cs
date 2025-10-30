namespace Minecraft_Realms_Emulator.Responses
{
    public class MinecraftServerQueryRepsonse
    {
        public MinecraftServerQueryVersionObject Version { get; set; } = null!;
        public bool EnforcesSecureChat { get; set; }
        public string Description { get; set; } = null!;
        public MinecraftServerQueryPlayersObject Players { get; set; } = null!;
    }

    public class MinecraftServerQueryVersionObject
    {
        public string Name { get; set; } = null!;
        public string Protocol { get; set; } = null!;
    }

    public class MinecraftServerQueryPlayersObject
    {
        public int Max { get; set; }
        public int Online { get; set; }
        public List<MinecraftServerQueryPlayersSampleObject> Sample { get; set; } = null!;
    }

    public class MinecraftServerQueryPlayersSampleObject
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}