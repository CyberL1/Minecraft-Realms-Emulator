namespace Core.Entities;

public class WorldSettings
{
    public int Id { get; set; }
    public bool Hardcore { get; set; }
    public required int SlotId { get; set; }
    public Slot Slot { get; set; } = null!;
}
