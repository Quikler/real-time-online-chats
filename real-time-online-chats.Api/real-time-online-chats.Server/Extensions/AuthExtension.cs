using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;

namespace real_time_online_chats.Server.Extensions;

public static class AuthExtensions
{
    public static Guid? GetUserId(this HttpContext httpContext)
    {
        if (httpContext.User.Identity is null || !httpContext.User.Identity.IsAuthenticated) return null;
        return Guid.TryParse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid guid) ? guid : null;
    }

    public static bool TryGetUserId(this HttpContext httpContext, out Guid userId)
    {
        userId = Guid.Empty;

        var res = httpContext.GetUserId();
        if (res is null) return false;

        userId = res.Value;
        return true;
    }

    public static void SetHttpOnlyRefreshToken(this HttpContext httpContext, string value)
    {
        httpContext.Response.Cookies.Append("refreshToken", value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        });
    }
}