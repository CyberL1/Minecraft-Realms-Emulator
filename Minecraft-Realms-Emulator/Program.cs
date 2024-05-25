using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Middlewares;
using Npgsql;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

if (Environment.GetEnvironmentVariable("CONNECTION_STRING") == null)
{
    Console.WriteLine("CONNECTION_STRING environment variable missing");
    return;
}

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataSourceBuilder = new NpgsqlDataSourceBuilder(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(dataSource);
});

var app = builder.Build();

// Initialize database
Database.Initialize(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DataContext>();

app.MapControllers();

var config = new ConfigHelper(db);
var mode = config.GetSetting(nameof(SettingsEnum.WorkMode));

if (mode == null)
{
    Console.WriteLine("Cannot get server work mode, exiting");
    Environment.Exit(1);
}

if (!Enum.IsDefined(typeof(WorkModeEnum), mode.Value))
{
    Console.WriteLine("Invalid server work mode, exiting");
    Environment.Exit(1);
}

if (mode.Value == nameof(WorkModeEnum.REALMS))
{
    var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

    foreach (var resourceName in resourceNames)
    {
        var path = $"{AppDomain.CurrentDomain.BaseDirectory}{resourceName.Replace("Minecraft_Realms_Emulator.Resources.", "").Replace(".", "/")}";
        
        var directory = Path.GetDirectoryName(path);
        var name = Path.GetFileName(path);

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(path))
        {
            using var file = new FileStream(path, FileMode.Create);
            stream.CopyTo(file);
        }
    }
}

var rewriteOptions = new RewriteOptions().AddRewrite(@"^(?!.*configuration)(.*)$", $"modes/{mode.Value}/$1", true);
app.UseRewriter(rewriteOptions);

app.UseMiddleware<MinecraftCookieMiddleware>();
app.UseMiddleware<CheckRealmOwnerMiddleware>();
app.UseMiddleware<ActiveSubscriptionMiddleware>();

Console.WriteLine($"Running in {mode.Value} mode");
app.Run();
