using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Middlewares
{
    public class ActiveSubscriptionMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext httpContext, DataContext db)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<CheckActiveSubscription>();

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

            var wId = int.Parse(worldId);
            var world = db.Worlds.Include(w => w.Subscription).Include(w => w.ParentWorld.Subscription)
                .FirstOrDefault(w => w.Id == wId);

            if (world == null)
            {
                throw new NullReferenceException("world is null");
            }

            if (world.ParentWorld != null)
            {
                world = world.ParentWorld;
            }

            if (world.Subscription == null)
            {
                throw new NullReferenceException("world.Subscription is null");
            }

            if (!attribute.IsSubscriptionActive(world.Subscription.StartDate))
            {
                var response = new ErrorResponse
                {
                    ErrorCode = 403,
                    ErrorMsg = "World is expired"
                };

                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsJsonAsync(response);
                return;
            }

            await next(httpContext);
        }
    }
}
