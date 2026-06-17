using System.Text.Json.Serialization;

namespace Core.Responses;

public class SlotOptions
{
    [JsonPropertyName("spawnProtection")] public int SpawnProtection { get; set; }

    [JsonPropertyName("forceGameMode")] public bool ForceGeeMode { get; set; }

    [JsonPropertyName("difficulty")] public int Difficulty { get; set; }

    [JsonPropertyName("gameMode")] public int GameMode { get; set; }

    [JsonPropertyName("slotName")] public required string SlotName { get; set; }

    [JsonPropertyName("version")] public required string Version { get; set; }

    [JsonPropertyName("compatibility")] public required string Compatibility { get; set; }
}
