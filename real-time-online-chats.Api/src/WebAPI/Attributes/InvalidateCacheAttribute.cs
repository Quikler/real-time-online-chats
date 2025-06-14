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

        switch (executionContext.Result)
        {
            // POST
            case CreatedAtActionResult created when created.Value is not null:
                await cacheService.CacheResponseAsync(template, created.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
                break;

            // PUT
            // PATCH
            case OkObjectResult ok when ok.Value is not null:
                await cacheService.CacheResponseAsync(template, ok.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
                break;

            // DELETE
            case NoContentResult noContent:
                await cacheService.RemoveCachedResponseAsync(template);
                break;
        }

        //switch (context.HttpContext.Request.Method)
        //{
            //case "POST":
                //if (executionContext.Result is not CreatedAtActionResult created || created.Value is null) return;
                //await cacheService.CacheResponseAsync(template, created.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
                //break;
            //case "PUT":
            //case "PATCH":
                //if (executionContext.Result is not OkObjectResult ok || ok.Value is null) return;
                //await cacheService.CacheResponseAsync(template, ok.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
                //break;
            //case "DELETE":
                //if (executionContext.Result is not NoContentResult noContent) return;
                //await cacheService.RemoveCachedResponseAsync(template);
                //break;
        //}
    }
}
