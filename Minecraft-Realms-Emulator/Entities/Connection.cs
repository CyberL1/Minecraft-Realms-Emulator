namespace Minecraft_Realms_Emulator.Entities
{
    public class Connection
    {
        public int Id { get; set; }
        public World World { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool PendingUpdate { get; set; }
    }
}
