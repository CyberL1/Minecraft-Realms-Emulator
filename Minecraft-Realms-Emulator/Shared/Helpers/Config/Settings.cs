using Minecraft_Realms_Emulator.Shared.Enums;

namespace Minecraft_Realms_Emulator.Shared.Helpers.Config
{
    public class Settings
    {
        public string DefaultServerAddress { get; set; } = "127.0.0.1";
        public string NewsLink { get; set; } = "https://github.com/CyberL1/Minecraft-Realms-Emulator";
        public bool TrialMode { get; set; } = true;
        public string WorkMode { get; set; } = nameof(WorkModeEnum.EXTERNAL);
        public bool OnlineMode { get; set; } = false;
        public bool AutomaticRealmsCreation { get; set; } = true;
    }
}
