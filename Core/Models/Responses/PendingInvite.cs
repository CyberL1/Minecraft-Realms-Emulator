namespace Core.Models.Responses;

public class PendingInvite
{
    public required string InvitationId { get; set; }
    public required string WorldName { get; set; }
    public required string WorldOwnerUuid { get; set; }
    public required long Date { get; set; }
}
