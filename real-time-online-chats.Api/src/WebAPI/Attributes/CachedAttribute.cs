using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Services.Cache;

namespace real_time_online_chats.Server.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CachedAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheConfig = context.HttpContext.RequestServices.GetRequiredService<IOptions<RedisCacheConfiguration>>().Value;
        if (!cacheConfig.Enabled)
        {
            await next();
            return;
        }

        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
        var cacheKey = context.HttpContext.Request.Path;
        var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

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

        var executedContext = await next();
        if (executedContext.Result is OkObjectResult ok && ok.Value is not null)
        {
            await cacheService.CacheResponseAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
        }
    }
}
