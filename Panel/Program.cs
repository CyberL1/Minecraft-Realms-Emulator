using Panel.Components;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

if (Environment.GetEnvironmentVariable("ADMIN_KEY") == null)
{
    Console.WriteLine("ADMIN_KEY enviornment variable not found, exiting");
    Environment.Exit(1);
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
