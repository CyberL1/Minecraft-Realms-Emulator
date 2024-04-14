using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Helpers
{
    public class Database
    {
        public static void Initialize(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            db.Database.Migrate();

            if (db.Configuration.FirstOrDefault(s => s.Key == "newsLink") == null)
            {
                var newsLink = new Configuration
                {
                    Key = "newsLink",
                    Value = "\"https://github.com/CyberL1/Minecraft-Realms-Emulator\""
                };

                db.Configuration.Add(newsLink);
            }

            if (db.Configuration.FirstOrDefault(s => s.Key == "defaultServerAddress") == null)
            {
                var defaultServerAddress = new Configuration
                {
                    Key = "defaultServerAddress",
                    Value = "\"127.0.0.1\""
                };

                db.Configuration.Add(defaultServerAddress);
            }

            if (db.Configuration.FirstOrDefault(x => x.Key == "trialMode") == null)
            {
                var trialMode = new Configuration
                {
                    Key = "trialMode",
                    Value = true
                };

                db.Configuration.Add(trialMode);
            }

            db.SaveChanges();
        }
    }
}
