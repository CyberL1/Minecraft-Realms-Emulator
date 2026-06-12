namespace Core.Middlewares;

public class AuthorizationMiddleware(RequestDelegate next)
{
    public Task Invoke(HttpContext httpContext)
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

        return next(httpContext);
    }
}
