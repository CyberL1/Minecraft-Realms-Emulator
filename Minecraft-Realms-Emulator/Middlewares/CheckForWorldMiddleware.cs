using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Middlewares
{
    public class CheckForWorldMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext httpContext, DataContext db)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<CheckForWorldAttribute>();

            if (attribute == null)
            {
                await next(httpContext);
                return;
            }

            var worldId = httpContext.Request.RouteValues["wId"]?.ToString();
            var world = db.Worlds.FirstOrDefault(w => worldId != null && w.Id == int.Parse(worldId));

            if (!attribute.WorldExists(world))
            {
                ErrorResponse errorResponse = new()
                {
                    ErrorCode = 404,
                    ErrorMsg = "World not found"
                };

                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsJsonAsync(errorResponse);
                return;
            }

            await next(httpContext);
        }
    }
}
