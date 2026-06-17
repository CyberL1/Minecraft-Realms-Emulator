namespace Core.Entities;

public class RealmConnection
{
    public int Id { get; set; }
    public int RealmId { get; set; }
    public Realm Realm { get; set; } = null!;
    public required string Address { get; set; }
    public string? ResourcePackUrl { get; set; }
    public string? ResourcePackHash { get; set; }
}