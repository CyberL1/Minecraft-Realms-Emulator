using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Shared.Enums;
using Minecraft_Realms_Emulator.Shared.Helpers;
using Minecraft_Realms_Emulator.Shared.Data;
using Minecraft_Realms_Emulator.Shared.Middlewares;
using Npgsql;
using System.Diagnostics;
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5192");
    });
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

app.UseCors();
app.MapControllers();

var mode = Environment.GetEnvironmentVariable("WORKMODE");

if (mode == null)
{
    Console.WriteLine("Cannot get server work mode, exiting");
    Environment.Exit(1);
}

if (!Enum.IsDefined(typeof(WorkModeEnum), mode))
{
    Console.WriteLine("Invalid server work mode, exiting");
    Environment.Exit(1);
}

if (mode == nameof(WorkModeEnum.REALMS))
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

    // Check if docker is running
    try
    {
        ProcessStartInfo dockerProcessInfo = new();
        dockerProcessInfo.FileName = "docker";
        dockerProcessInfo.Arguments = "info";

        Process dockerProcess = new();
        dockerProcess.StartInfo = dockerProcessInfo;
        dockerProcess.Start();

        dockerProcess.WaitForExit();

        if (dockerProcess.ExitCode != 0)
        {
            Console.WriteLine("Docker is required to run in REALMS work mode, but its daemon is not running.");
            Environment.Exit(1);
        }
    }
    catch
    {
        Console.WriteLine("Docker is required to run in REALMS work mode, but it is not installed");
        Console.WriteLine("You can install it here: https://docs.docker.com/engine/install");
        Environment.Exit(1);
    }
}

var rewriteOptions = new RewriteOptions().AddRewrite(@"^(?!api)(.*)$", $"modes/{mode}/$1", true);
app.UseRewriter(rewriteOptions);

app.UseMiddleware<MinecraftCookieMiddleware>();
app.UseMiddleware<CheckRealmOwnerMiddleware>();
app.UseMiddleware<ActiveSubscriptionMiddleware>();
app.UseMiddleware<AdminKeyMiddleware>();
app.UseMiddleware<CheckForWorldMiddleware>();
app.UseMiddleware<RouteLoggingMiddleware>();

Console.WriteLine($"Running in {mode} mode");
app.Run();
