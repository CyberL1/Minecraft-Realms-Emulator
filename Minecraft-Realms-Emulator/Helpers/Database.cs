﻿using Microsoft.EntityFrameworkCore;
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

            foreach (var property in settings.GetType().GetProperties())
            {
                var name = property.Name;
                var value = property.GetValue(settings);

                if (config.GetSetting(name) == null)
                {
                    db.Configuration.Add(new Configuration
                    {
                        Key = name,
                        Value = JsonConvert.SerializeObject(value)
                    });
                }
            }

            db.SaveChanges();
        }
    }
}
