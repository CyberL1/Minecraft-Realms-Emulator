namespace Minecraft_Realms_Emulator.Entities
{
    public class Invite
    {
        public int Id { get; set; }
        public required string InvitationId { get; set; }
        public required string RecipientUuid { get; set; }
        public required World World { get; set; }
        public DateTime Date { get; set; }
    }
}
