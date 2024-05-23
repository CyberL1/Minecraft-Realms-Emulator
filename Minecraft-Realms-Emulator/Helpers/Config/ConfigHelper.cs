using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Newtonsoft.Json;

namespace Minecraft_Realms_Emulator.Helpers
{
    public class ConfigHelper
    {
        private readonly DataContext Db;

        public ConfigHelper(DataContext db)
        {
            Db = db;
        }

        public List<Configuration> GetSettings()
        {
            List<Configuration> settings = [];

            foreach (var setting in Db.Configuration)
            {
                Configuration settingTyped = new()
                {
                    Key = setting.Key,
                    Value = JsonConvert.DeserializeObject(setting.Value)
                };

                settings.Add(settingTyped);
            }

            return settings;
        }

        public Configuration? GetSetting(string key)
        {
            var setting = Db.Configuration.Find(FirstCharToLowerCase(key));

            if (setting == null) return null;

            return new()
            {
                Key = setting.Key,
                Value = JsonConvert.DeserializeObject(setting.Value)
            };
        }

        private static string FirstCharToLowerCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

            return str;
        }
    }
}
