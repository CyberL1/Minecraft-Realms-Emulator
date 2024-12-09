using Minecraft_Realms_Emulator.Shared.Attributes;

namespace Minecraft_Realms_Emulator.Shared.Middlewares
{
    public class AdminKeyMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<RequireAdminKeyAttribute>();

            if (attribute == null)
            {
                await _next(httpContext);
                return;
            }

            if (!attribute.HasAdminKey(httpContext.Request.Headers.Authorization))
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("You don't have access to this resource");
                return;
            }

            await _next(httpContext);
        }
    }
}
