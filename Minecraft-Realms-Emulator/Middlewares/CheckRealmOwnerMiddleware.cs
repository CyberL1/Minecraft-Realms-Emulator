using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;

namespace Minecraft_Realms_Emulator.Middlewares
{
    public class CheckRealmOwnerMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext httpContext, DataContext db)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<CheckRealmOwnerAttribute>();

            if (attribute == null)
            {
                await next(httpContext);
                return;
            }

            var worldId = httpContext.Request.RouteValues["wId"]?.ToString();

            if (worldId == null)
            {
                return;
            }

            var playerUuid = httpContext.Request.Headers.Cookie.ToString().Split(";")[0].Split(":")[2];
            var world = db.Worlds.Include(w => w.ParentWorld).FirstOrDefault(w => w.Id == int.Parse(worldId));

            if (world != null && !attribute.IsRealmOwner(playerUuid,
                    world.ParentWorld == null ? world.OwnerUUID : world.ParentWorld.OwnerUUID))
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("You don't own this world");
                return;
            }

            await next(httpContext);
        }
    }
}
