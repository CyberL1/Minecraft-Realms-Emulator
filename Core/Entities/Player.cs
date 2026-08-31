using System.Text.Json.Serialization;

namespace Core.Entities;

public class Player
{
    public int Id { get; set; }
    public required string Uuid { get; set; }
    public required string Name { get; set; }
    public required bool Operator { get; set; }
    public required bool Owner { get; set; }
    public required bool Accepted { get; set; }
    [JsonIgnore] public int? RealmId { get; set; }
    [JsonIgnore] public Realm Realm { get; set; } = null!;
}