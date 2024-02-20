namespace Minecraft_Realms_Emulator.Entities
{
    public class Invite
    {
        public int Id { get; set; }
        public string InvitationId { get; set; }= string.Empty;
        public string RecipeintUUID { get; set; } = string.Empty;
        public World World { get; set; }
        public DateTime Date { get; set; }
    }
}
