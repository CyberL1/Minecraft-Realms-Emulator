using Core.Attributes;
using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Middlewares;

public class CheckRealmAccessMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, DataContext db, CookiePlayerData playerData)
    {
        var attribute = httpContext.GetEndpoint()?.Metadata.GetMetadata<HasRealmAccessAttribute>();

        var realmIdValue = httpContext.GetRouteData().Values["realmId"];

        if (attribute == null || realmIdValue == null)
        {
            await next(httpContext);
            return;
        }

        var realmId = int.Parse(realmIdValue.ToString()!);
        var realm = await db.Realms.Include(realm => realm.Players).FirstOrDefaultAsync(realm => realm.Id == realmId);

        if (realm == null)
        {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsJsonAsync(ApiError.WorldNotFound);

            return;
        }

        if (realm.Players.All(player => player.Uuid != playerData.Uuid))
        {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsJsonAsync(ApiError.NotAWorldMember);

            return;
        }

        if (attribute.IsOwner && realm.Owner.Uuid.Replace("-", "") != playerData.Uuid)
        {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsJsonAsync(ApiError.NotOwner);

            return;
        }

        await next(httpContext);
    }
}
