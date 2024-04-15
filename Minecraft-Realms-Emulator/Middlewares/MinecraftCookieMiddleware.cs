using Minecraft_Realms_Emulator.Attributes;
using System.Text;

namespace Minecraft_Realms_Emulator.Middlewares
{
    public class MinecraftCookieMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<RequireMinecraftCookieAttribute>();

            if (attribute == null)
            {
                await _next(httpContext);
                return;
            }

            if (httpContext.Request.Headers.Cookie.ToString() == "")
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Authorization required"));
                return;
            }

            string cookie = httpContext.Request.Headers.Cookie.ToString();
            if (!attribute.HasMinecraftCookie(cookie)) throw new UnauthorizedAccessException("Malformed cookie header");

            await _next(httpContext);
        }
    }
}
