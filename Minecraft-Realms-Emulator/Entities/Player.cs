using System.Text.Json.Serialization;

namespace Minecraft_Realms_Emulator.Entities
{
    public class Player
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;
        [JsonPropertyName("operator")]
        public bool Operator { get; set; }
        [JsonPropertyName("accepted")]
        public bool Accepted { get; set; }
        [JsonPropertyName("online")]
        public bool Online { get; set; }
        [JsonPropertyName("permission")]
        public string Permission { get; set; } = "MEMBER";
    }
}
