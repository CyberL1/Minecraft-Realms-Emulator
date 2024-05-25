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

            Subscription subscription = db.Subscriptions.First(s => s.World.Id == int.Parse(httpContext.Request.RouteValues["wId"].ToString()));
            Console.WriteLine(attribute.IsSubscriptionActive(subscription.StartDate));

            if (!attribute.IsSubscriptionActive(subscription.StartDate))
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("You don't have an active subscription for this world");
                return;
            }

            await _next(httpContext);
        }
    }
}
