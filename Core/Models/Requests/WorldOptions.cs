using Core.Enums;

namespace Core.Models.Requests;

public class WorldOptions
{
    public required int SlotId { get; set; }
    public required int SpawnProtection { get; set; }
    public required bool ForceGameMode { get; set; }
    public required SlotOptionsDifficulty Difficulty { get; set; }
    public required SlotOptionsGameMode GameMode { get; set; }
    public required string SlotName { get; set; }
    public required string Version { get; set; }
    public required string Compatibility { get; set; }
    public long? WoldTemplateId { get; set; }
    public long? WorldTemplateImage { get; set; }
    public required bool Hardcore { get; set; }
}
