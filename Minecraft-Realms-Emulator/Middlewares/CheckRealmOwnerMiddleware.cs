using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Middlewares
{
    public class CheckRealmOwnerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext, DataContext db)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<CheckRealmOwnerAttribute>();

            if (attribute == null)
            {
                await _next(httpContext);
                return;
            }

            string playerUUID = httpContext.Request.Headers.Cookie.ToString().Split(";")[0].Split(":")[2];
            World world = db.Worlds.Find(int.Parse(httpContext.Request.RouteValues["wId"].ToString()));

            if (world != null && !attribute.IsRealmOwner(playerUUID, world.OwnerUUID))
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("You don't own this world");
                return;
            }

            await _next(httpContext);
        }
    }
}
