namespace Core.Entities;

public class Realm
{
    public int Id { get; set; }
    public required Subscription Subscription { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string State { get; set; }
    public required Player Owner { get; set; }
    public required List<Player> Players { get; set; }
    public required List<Slot> Slots { get; set; }
    public required string WorldType { get; set; }
    public required Slot ActiveSlot { get; set; }
    public Realm? ParentWorld { get; set; }
}
