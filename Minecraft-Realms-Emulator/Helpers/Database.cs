using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
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

            if (config.GetSetting("newsLink") == null)
            {
                db.Configuration.Add(new Configuration
                {
                    Key = "newsLink",
                    Value = JsonConvert.SerializeObject(settings.NewsLink)
                });
            }
            
            if (config.GetSetting("defaultServerAddress") == null)
            {
                db.Configuration.Add(new Configuration
                {
                    Key = "defaultServerAddress",
                    Value = JsonConvert.SerializeObject(settings.DefaultServerAddress)
                });
            }

            if (config.GetSetting("trialMode") == null)
            {
                db.Configuration.Add(new Configuration
                {
                    Key = "trialMode",
                    Value = JsonConvert.SerializeObject(settings.TrialMode)
                });
            }

            db.SaveChanges();
        }
    }
}
