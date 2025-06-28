using System.Security.Claims;

namespace real_time_online_chats.Server.Extensions;

public static class AuthExtensions
{
    public static bool TryGetUserId(this HttpContext httpContext, out Guid userId) => httpContext.User.TryGetUserId(out userId);

    public static Guid? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is null || !claimsPrincipal.Identity.IsAuthenticated) return null;
        return Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out Guid guid) ? guid : null;
    }

    public static bool TryGetUserId(this ClaimsPrincipal claimsPrincipal, out Guid userId)
    {
        userId = Guid.Empty;

        var res = claimsPrincipal.GetUserId();
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
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.Add(TimeSpan.FromDays(180)),
        });
    }
}