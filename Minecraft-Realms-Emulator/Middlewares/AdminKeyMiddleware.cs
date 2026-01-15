using Microsoft.Extensions.Options;
using Minecraft_Realms_Emulator.Attributes;

namespace Minecraft_Realms_Emulator.Middlewares
{
    public class AdminKeyMiddleware(RequestDelegate next, IOptions<AppSettings.AppSettings> appSettings)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<RequireAdminKeyAttribute>();

            if (attribute == null)
            {
                await next(httpContext);
                return;
            }

            var authorization = httpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authorization) || authorization != appSettings.Value.AdminKey)
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("You don't have access to this resource");
                return;
            }

            await next(httpContext);
        }
    }
}
