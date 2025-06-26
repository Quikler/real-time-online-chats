using Microsoft.AspNetCore.Mvc.Filters;
using real_time_online_chats.Server.Services.Cache;

namespace real_time_online_chats.Server.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RemoveCacheAttribute(string[] templates) : Attribute, IAsyncActionFilter
{
    public RemoveCacheAttribute(string template) : this([template]) { }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        foreach (var template in templates)
        {
            var currentTemplate = template;
            foreach (var (key, value) in context.ActionArguments)
            {
                currentTemplate = currentTemplate.Replace($"{{{key}}}", value?.ToString());
            }

            Console.WriteLine($"[RemoveCache]: {currentTemplate}");

            await cacheService.RemoveCachedResponseAsync(currentTemplate);
        }
    }
}
