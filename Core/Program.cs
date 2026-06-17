using Core.Data;
using Core.Middlewares;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using AppConfig = Core.Helpers.AppConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<CookiePlayerData>();

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetValue<string>("Database");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("No database specified. Ensure 'Database' field exists in appsettings.json file.");
    return;
}

builder.Services.AddDbContextPool<DataContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

AppConfig.InitializeAppConfig(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseRouting();

app.UseMiddleware<AuthorizationMiddleware>(app.Services);
app.UseMiddleware<CheckRealmAccessMiddleware>();

app.MapControllers();

app.Run();
