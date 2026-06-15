namespace Core.Entities;

public class Realm
{
    public int Id { get; set; }
    public Subscription Subscription { get; set; } = null!;
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string State { get; set; }
    public required List<Player> Players { get; set; }
    public required List<Slot> Slots { get; set; }
    public required string WorldType { get; set; }
    public required int ActiveSlotId { get; set; }
    public Slot ActiveSlot { get; set; } = null!;
    public Realm? ParentWorld { get; set; }
}
