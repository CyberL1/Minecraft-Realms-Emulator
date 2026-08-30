namespace Core.Entities;

public class PendingInvite
{
    public int Id { get; set; }
    public required string InvitationId { get; set; }
    public required DateTime Date { get; set; }
    public int? RealmId { get; set; }
    public required Realm Realm { get; set; }
}