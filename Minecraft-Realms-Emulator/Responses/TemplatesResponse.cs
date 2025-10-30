using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Responses
{
    public class TemplatesResponse
    {
        public List<Template> Templates { get; set; } = null!;
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
    }
}
