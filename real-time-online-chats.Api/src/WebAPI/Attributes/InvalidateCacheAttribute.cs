using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using real_time_online_chats.Server.Services.Cache;

namespace real_time_online_chats.Server.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class InvalidateCacheAttribute(string[] templates, int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
    public InvalidateCacheAttribute(string template, int timeToLiveSeconds) : this([template], timeToLiveSeconds) { }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executionContext = await next();
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        foreach (var template in templates)
        {
            var currentTemplate = template;
            foreach (var (key, value) in context.ActionArguments)
            {
                currentTemplate = currentTemplate.Replace($"{{{key}}}", value?.ToString());
            }

            Console.WriteLine($"[InvalidateCache]: {currentTemplate}");

            switch (executionContext.Result)
            {
                // POST
                case CreatedAtActionResult created when created.Value is not null:
                    await cacheService.CacheResponseAsync(currentTemplate, created.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
                    break;

                // PUT
                // PATCH
                case OkObjectResult ok when ok.Value is not null:
                    await cacheService.CacheResponseAsync(currentTemplate, ok.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
                    break;

                // DELETE
                case NoContentResult:
                    await cacheService.RemoveCachedResponseAsync(currentTemplate);
                    break;
            }
        }
    }
}
