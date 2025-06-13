using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using real_time_online_chats.Server.Services.Cache;

namespace real_time_online_chats.Server.Attributes;

public class InvalidateCacheAttribute(string template, int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executionContext = await next();
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        foreach (var (key, value) in context.ActionArguments)
        {
            template = template.Replace($"{{{key}}}", value?.ToString());
        }

        switch (context.HttpContext.Request.Method)
        {
            case "POST":
                break;
            case "PUT":
                if (executionContext.Result is not OkObjectResult ok || ok.Value is null) return;
                await cacheService.CacheResponseAsync(template, ok.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
                break;
            case "PATCH":
                break;
            case "DELETE":
                break;
        }
    }
}
