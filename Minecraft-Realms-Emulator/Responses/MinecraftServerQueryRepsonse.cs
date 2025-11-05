namespace Minecraft_Realms_Emulator.Responses
{
    public class MinecraftServerQueryRepsonse
    {
        public required MinecraftServerQueryVersionObject Version { get; set; }
        public bool EnforcesSecureChat { get; set; }
        public required string Description { get; set; }
        public required MinecraftServerQueryPlayersObject Players { get; set; }
    }

    public class MinecraftServerQueryVersionObject
    {
        public required string Name { get; set; }
        public required string Protocol { get; set; }
    }

    public class MinecraftServerQueryPlayersObject
    {
        public int Max { get; set; }
        public int Online { get; set; }
        public required List<MinecraftServerQueryPlayersSampleObject> Sample { get; set; }
    }

    public class MinecraftServerQueryPlayersSampleObject
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
    }
}