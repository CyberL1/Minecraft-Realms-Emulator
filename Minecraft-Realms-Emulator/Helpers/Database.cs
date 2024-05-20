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

            if (!db.Configuration.Any(s => s.Key == "newsLink"))
            {
                var newsLink = new Configuration
                {
                    Key = "newsLink",
                    Value = "\"https://github.com/CyberL1/Minecraft-Realms-Emulator\""
                };

                db.Configuration.Add(newsLink);
            }

            if (!db.Configuration.Any(s => s.Key == "defaultServerAddress"))
            {
                var defaultServerAddress = new Configuration
                {
                    Key = "defaultServerAddress",
                    Value = "\"127.0.0.1\""
                };

                db.Configuration.Add(defaultServerAddress);
            }

            if (!db.Configuration.Any(x => x.Key == "trialMode"))
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
