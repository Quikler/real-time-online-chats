using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Services.Cache;

namespace real_time_online_chats.Server.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CachedByTemplateAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheConfig = context.HttpContext.RequestServices.GetRequiredService<IOptions<RedisCacheConfiguration>>().Value;
        if (!cacheConfig.Enabled)
        {
            await next();
            return;
        }

        var redisCacheService = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheService>();
        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

        var cachedResponse = await redisCacheService.GetCachedResponseAsync(cacheKey);

        if (!string.IsNullOrWhiteSpace(cachedResponse))
        {
            context.Result = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200,
            };
            return;
        }

        var cachedByTemplateAttributeLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CachedByTemplateAttribute>>();
        cachedByTemplateAttributeLogger.LogInformation("Cache by template key: {cacheKey}", cacheKey);

        var executedContext = await next();
        if (executedContext.Result is OkObjectResult ok && ok.Value is not null)
        {
            await redisCacheService.CacheResponseAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
        }
    }

    private static string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var sb = new StringBuilder();
        sb.Append($"{request.Path}");

        foreach (var (key, value) in request.Query.OrderBy(q => q.Key))
        {
            sb.Append($"|{key}-{value}");
        }

        return sb.ToString();
    }
}
