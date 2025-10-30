namespace Minecraft_Realms_Emulator.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public bool Operator { get; set; }
        public bool Accepted { get; set; }
        public bool Online { get; set; }
        public string Permission { get; set; } = "MEMBER";
        public World World { get; set; }
    }
}
