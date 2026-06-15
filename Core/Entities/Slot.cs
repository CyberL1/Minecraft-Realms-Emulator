namespace Core.Entities;

public class Slot
{
    public int Id { get; set; }
    public required int SlotId { get; set; }
    public required SlotOptions Options { get; set; }
    public required WorldSettings Settings { get; set; }
}
