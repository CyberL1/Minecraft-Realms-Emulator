namespace Minecraft_Realms_Emulator.Responses
{
    public class InviteList
    {
        public List<InviteResponse> Invites { get; set; }
    }

    public class InviteResponse
    {
        public string InvitationId { get; set; } = string.Empty;
        public string WorldName { get; set; } = string.Empty;
        public string WorldOwnerName { get; set; } = string.Empty;
        public string WorldOwnerUuid { get; set; } = string.Empty;
        public long Date { get; set; }
    }
}
