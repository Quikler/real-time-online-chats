using Microsoft.AspNetCore.Mvc.Filters;
using real_time_online_chats.Server.Services.Cache;

namespace real_time_online_chats.Server.Attributes;

public class RemoveCacheAttribute(string template) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executionContext = await next();
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        foreach (var (key, value) in context.ActionArguments)
        {
            template = template.Replace($"{{{key}}}", value?.ToString());
        }

        await cacheService.RemoveCachedResponseAsync(template);
    }
}
