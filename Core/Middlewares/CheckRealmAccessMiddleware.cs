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
            var apiError = new ApiError
            {
                ErrorCode = 404,
                ErrorMsg = "World not found"
            };

            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsJsonAsync(apiError);

            return;
        }

        if (realm.Players.All(player => player.Uuid != playerData.Uuid))
        {
            var apiError = new ApiError // TODO: Check if this is correct
            {
                ErrorCode = 403,
                ErrorMsg = "Missing access"
            };

            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsJsonAsync(apiError);

            return;
        }

        if (attribute.IsOwner && realm.Players.First().Uuid.Replace("-", "") != playerData.Uuid)
        {
            var apiError = new ApiError
            {
                ErrorCode = 403,
                ErrorMsg = "Not owner"
            };

            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsJsonAsync(apiError);

            return;
        }

        await next(httpContext);
    }
}
