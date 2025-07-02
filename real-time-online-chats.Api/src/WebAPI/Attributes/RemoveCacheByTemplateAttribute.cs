using Microsoft.AspNetCore.Mvc.Filters;
using real_time_online_chats.Server.Services.Cache;

namespace real_time_online_chats.Server.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RemoveCacheByTemplateAttribute(string[] templates, string contentAfterTemplate = "") : Attribute, IAsyncActionFilter
{
    public RemoveCacheByTemplateAttribute(string template, string contentAfterTemplate = "") : this([template], contentAfterTemplate) { }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();

        var redisCacheService = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheService>();
        var removeCacheAttributeLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RemoveCacheAttribute>>();

        foreach (var template in templates)
        {
            var currentTemplate = template;
            foreach (var (key, value) in context.ActionArguments)
            {
                currentTemplate = currentTemplate.Replace($"{{{key}}}", value?.ToString());
            }

            currentTemplate += contentAfterTemplate;
            removeCacheAttributeLogger.LogInformation("{currentTemplate}", currentTemplate);
            await redisCacheService.RemoveCachedByTemplateAsync(currentTemplate);
        }
    }
}
