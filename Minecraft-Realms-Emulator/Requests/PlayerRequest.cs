namespace Minecraft_Realms_Emulator.Requests
{
    public class PlayerRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public bool Operator { get; set; }
        public bool Accepted { get; set; }
        public bool Online { get; set; }
        public string Permission { get; set; } = "MEMBER";
    }
}
