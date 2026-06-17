using System.Security.Cryptography;
using Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Core.Middlewares;

public class AuthorizationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
{
    public async Task Invoke(HttpContext httpContext, CookiePlayerData playerData)
    {
        var cookieHeader = httpContext.Request.Headers.Cookie.ToString();

        if (cookieHeader.Trim() == "")
        {
            httpContext.Response.StatusCode = 401;
            await Task.CompletedTask;

            return;
        }

        var hasSid = httpContext.Request.Cookies.ContainsKey("sid");
        var hasUser = httpContext.Request.Cookies.ContainsKey("user");
        var hasVersion = httpContext.Request.Cookies.ContainsKey("version");

        if (!(hasSid && hasUser && hasVersion))
        {
            httpContext.Response.StatusCode = 401;
            await Task.CompletedTask;

            return;
        }

        var token = new JsonWebToken(httpContext.Request.Cookies["sid"]!.Split(":")[1]);
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateActor = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "authentication",
            IssuerSigningKeyResolver = (_, _, _, _) =>
            {
                return memoryCache.GetOrCreate("minecraftPublicKeys", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                    var minecraftPublicKeys = new HttpClient()
                        .GetFromJsonAsync<MinecraftPublicKeys>("https://api.minecraftservices.com/publickeys")
                        .GetAwaiter()
                        .GetResult();

                    var signingKeys = new List<SecurityKey>();

                    if (minecraftPublicKeys == null)
                    {
                        Console.WriteLine("Couldn't fetch Minecraft Public Keys");
                        return signingKeys;
                    }

                    var publicKeyBytes = Convert.FromBase64String(minecraftPublicKeys.AuthenticationKeys[0].PublicKey);
                    var rsa = RSA.Create();

                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                    signingKeys.Add(new RsaSecurityKey(rsa.ExportParameters(false)));

                    return signingKeys;
                });
            }
        };

        var validationResult = await new JsonWebTokenHandler().ValidateTokenAsync(token, validationParameters);

        if (!validationResult.IsValid)
        {
            httpContext.Response.StatusCode = 401;
            await Task.CompletedTask;

            return;
        }

        playerData.Uuid = httpContext.Request.Cookies["sid"]!.Split(":")[2];
        playerData.Name = httpContext.Request.Cookies["user"]!;
        playerData.Version = httpContext.Request.Cookies["version"]!;

        await next(httpContext);
    }
}
