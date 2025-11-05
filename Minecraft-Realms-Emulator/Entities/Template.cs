namespace Minecraft_Realms_Emulator.Entities
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string Trailer { get; set; } = string.Empty;
        public string RecommendedPlayers { get; set; } = string.Empty;
        public required string Type { get; set; }
    }
}