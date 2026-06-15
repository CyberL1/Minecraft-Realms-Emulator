namespace Core.Entities;

public class SlotOptions
{
    public int Id { get; set; }
    public int SpawnProtection { get; set; }
    public bool ForceGamemode { get; set; }
    public int Difficulty { get; set; }
    public int Gamemode { get; set; }
    public required string SlotName { get; set; }
    public required string Version { get; set; }
}
