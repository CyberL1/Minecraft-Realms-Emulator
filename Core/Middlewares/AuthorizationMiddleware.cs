using Core.Models;

namespace Core.Middlewares;

public class AuthorizationMiddleware(RequestDelegate next)
{
    public Task Invoke(HttpContext httpContext, CookiePlayerData playerData)
    {
        var cookieHeader = httpContext.Request.Headers.Cookie.ToString();

        if (cookieHeader.Trim() == "")
        {
            httpContext.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        var hasSid = httpContext.Request.Cookies.ContainsKey("sid");
        var hasUser = httpContext.Request.Cookies.ContainsKey("user");
        var hasVersion = httpContext.Request.Cookies.ContainsKey("version");

        if (!(hasSid && hasUser && hasVersion))
        {
            httpContext.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        playerData.Uuid = httpContext.Request.Cookies["sid"]!.Split(":")[2];
        playerData.Name = httpContext.Request.Cookies["user"]!;
        playerData.Version = httpContext.Request.Cookies["version"]!;

        return next(httpContext);
    }
}
