namespace Core.Entities;

public class Player
{
    public int Id { get; set; }
    public required string Uuid { get; set; }
    public required string Name { get; set; }
    public required bool Operator { get; set; }
    public required bool Accepted { get; set; }
    public int? RealmId { get; set; }
    public Realm Realm { get; set; } = null!;
}
