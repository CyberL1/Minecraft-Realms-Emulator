using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Middlewares;
using Npgsql;

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

app.UseMiddleware<MinecraftCookieMiddleware>();
app.UseMiddleware<CheckRealmOwnerMiddleware>();

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

var rewriteOptions = new RewriteOptions().AddRewrite(@"^(?!.*configuration)(.*)$", $"modes/{mode.Value}/$1", true);
app.UseRewriter(rewriteOptions);

Console.WriteLine($"Running in {mode.Value} mode");
app.Run();
