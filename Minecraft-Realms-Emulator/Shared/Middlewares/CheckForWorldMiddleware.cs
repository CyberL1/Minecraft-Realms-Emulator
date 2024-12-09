using Minecraft_Realms_Emulator.Shared.Attributes;
using Minecraft_Realms_Emulator.Shared.Data;
using Minecraft_Realms_Emulator.Shared.Entities;
using Minecraft_Realms_Emulator.Shared.Responses;

namespace Minecraft_Realms_Emulator.Shared.Middlewares
{
    public class CheckForWorldMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext, DataContext db)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<CheckForWorldAttribute>();

            if (attribute == null)
            {
                await _next(httpContext);
                return;
            }

            World world = db.Worlds.FirstOrDefault(w => w.Id == int.Parse(httpContext.Request.RouteValues["wId"].ToString()));

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

            await _next(httpContext);
        }
    }
}
