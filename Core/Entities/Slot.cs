namespace Core.Entities;

public class Slot
{
    public int Id { get; set; }
    public required int SlotId { get; set; }
    public SlotOptions Options { get; set; } = null!;
    public required WorldSettings Settings { get; set; }
    public int? RealmId { get; set; }
    public Realm Realm { get; set; } = null!;
}
