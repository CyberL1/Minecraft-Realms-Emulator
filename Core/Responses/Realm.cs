using System.Diagnostics.CodeAnalysis;
using Core.Entities;

namespace Core.Responses;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Realm
{
    public required int Id { get; set; }
    public required string RemoteSubscriptionId { get; set; }
    public required string Name { get; set; }
    public required string Motd { get; set; }
    public required string State { get; set; }
    public required string Owner { get; set; }
    public required string OwnerUUID { get; set; }
    public List<Player>? Players { get; set; }
    public List<Slot>? Slots { get; set; }
    public required bool Expired { get; set; }
    public required bool ExpiredTrial { get; set; }
    public required int DaysLeft { get; set; }
    public string? WorldType { get; set; }
    public required bool IsHardcore { get; set; }
    public required int GameMode { get; set; }
    public required int ActiveSlot { get; set; }
    public int? ParentWorldId { get; set; }
    public string? ParentWorldName { get; set; }
    public required string ActiveVersion { get; set; }
    public required string Compatibility { get; set; }
}
