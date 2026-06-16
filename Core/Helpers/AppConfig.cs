using System.Text.Json;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Helpers;

public class AppConfig
{
    private static readonly AppConfig AppConfigTyped = new();

    public static void InitializeAppConfig(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();

        db.Database.Migrate();

        var appConfigModel = new Models.AppConfig();
        var properties = appConfigModel.GetType().GetProperties();

        var defaultConfig = properties.Select(property => new Entities.AppConfig
        {
            Key = property.Name,
            Value = JsonSerializer.Serialize(property.GetValue(appConfigModel))
        });

        foreach (var config in defaultConfig)
            db.Database.ExecuteSqlInterpolated(
                $"""INSERT INTO "AppConfig" ("Key", "Value") VALUES ({config.Key}, {config.Value}::jsonb) ON CONFLICT ("Key") DO NOTHING""");

        var dbConfigs = db.AppConfig.ToDictionary(p => p.Key, p => p.Value);

        foreach (var property in properties)
            if (dbConfigs.TryGetValue(property.Name, out var dbJsonValue) && dbJsonValue != null)
            {
                var deserializedValue = JsonSerializer.Deserialize(dbJsonValue, property.PropertyType);
                property.SetValue(AppConfigTyped, deserializedValue);
            }
    }
}
