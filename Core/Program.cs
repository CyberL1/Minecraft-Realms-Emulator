using AuthorizationMiddleware = Core.Middlewares.AuthorizationMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseRouting();

app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();
