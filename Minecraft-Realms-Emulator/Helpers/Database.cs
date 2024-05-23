using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers.Config;
using Newtonsoft.Json;

namespace Minecraft_Realms_Emulator.Helpers
{
    public class Database
    {
        public static void Initialize(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            db.Database.Migrate();

            var config = new ConfigHelper(db);
            var settings = new Settings();

            if (config.GetSetting(nameof(SettingsEnum.newsLink)) == null)
            {
                db.Configuration.Add(new Configuration
                {
                    Key = nameof(SettingsEnum.newsLink),
                    Value = JsonConvert.SerializeObject(settings.NewsLink)
                });
            }
            
            if (config.GetSetting(nameof(SettingsEnum.defaultServerAddress)) == null)
            {
                db.Configuration.Add(new Configuration
                {
                    Key = nameof(SettingsEnum.defaultServerAddress),
                    Value = JsonConvert.SerializeObject(settings.DefaultServerAddress)
                });
            }

            if (config.GetSetting(nameof(SettingsEnum.trialMode)) == null)
            {
                db.Configuration.Add(new Configuration
                {
                    Key = nameof(SettingsEnum.trialMode),
                    Value = JsonConvert.SerializeObject(settings.TrialMode)
                });
            }

            db.SaveChanges();
        }
    }
}
