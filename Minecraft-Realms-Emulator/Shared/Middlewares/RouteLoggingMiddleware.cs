namespace Minecraft_Realms_Emulator.Shared.Middlewares
{
    public class RouteLoggingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext)
        {
            Console.WriteLine($"{httpContext.Request.Method} {httpContext.Request.Path}{httpContext.Request.QueryString}");

            await _next(httpContext);
        }
    }
}
