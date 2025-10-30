using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Middlewares
{
    public class ActiveSubscriptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext, DataContext db)
        {
            var endpoint = httpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<CheckActiveSubscription>();

            if (attribute == null)
            {
                await _next(httpContext);
                return;
            }

            var wId = int.Parse(httpContext.Request.RouteValues["wId"].ToString());
            World world = db.Worlds.Include(w => w.Subscription).Include(w => w.ParentWorld.Subscription).FirstOrDefault(w => w.Id == wId);

            if (world.ParentWorld?.Subscription != null)
            {
                world = world.ParentWorld;
            }

            if (!attribute.IsSubscriptionActive(world.Subscription.StartDate))
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("You don't have an active subscription for this world");
                return;
            }

            await _next(httpContext);
        }
    }
}
