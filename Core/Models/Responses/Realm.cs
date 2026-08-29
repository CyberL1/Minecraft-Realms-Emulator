using System.Diagnostics.CodeAnalysis;

namespace Core.Models.Responses;

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
    public IEnumerable<Player>? Players { get; set; }
    public IEnumerable<Slot>? Slots { get; set; }
    public required bool Expired { get; set; }
    public required bool ExpiredTrial { get; set; }
    public required int DaysLeft { get; set; }
    public required string WorldType { get; set; }
    public required bool IsHardcore { get; set; }
    public required int GameMode { get; set; }
    public required int ActiveSlot { get; set; }
    public int? ParentWorldId { get; set; }
    public string? ParentWorldName { get; set; }
    public required string ActiveVersion { get; set; }
    public required string Compatibility { get; set; }
    public RealmRegionSelectionPreference? RegionSelectionPreference { get; set; }
}
