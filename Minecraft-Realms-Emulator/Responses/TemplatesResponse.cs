using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Responses
{
    public class TemplatesResponse
    {
        public required List<Template> Templates { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
    }
}
