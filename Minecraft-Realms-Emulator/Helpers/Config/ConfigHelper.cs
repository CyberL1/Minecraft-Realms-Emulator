using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Data;
using Newtonsoft.Json;

namespace Minecraft_Realms_Emulator.Helpers.Config
{
    public abstract class ConfigHelper
    {
        private static readonly Dictionary<string, dynamic> ConfigCache = new();

        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            db.Database.Migrate();
            var settings = new Settings();


            foreach (var property in settings.GetType().GetProperties())
            {
                var name = property.Name;
                var value = property.GetValue(settings);

                if (db.Configuration.Find(name) == null)
                {
                    db.Configuration.Add(new Configuration
                    {
                        Key = name,
                        Value = JsonConvert.SerializeObject(value)
                    });
                }
            }

            db.SaveChanges();

            foreach (var setting in db.Configuration)
            {
                ConfigCache[setting.Key] = JsonConvert.DeserializeObject(setting.Value);
            }
        }

        public static Dictionary<string, dynamic> GetConfig()
        {
            return ConfigCache;
        }

        public static dynamic GetSetting(string key)
        {
            return ConfigCache[key];
        }
    }
}