namespace Core.Models.Responses;

public class Slot
{
    public required int SlotId { get; set; }
    public required string Options { get; set; }
    public required List<SlotSetting> Settings { get; set; }
}