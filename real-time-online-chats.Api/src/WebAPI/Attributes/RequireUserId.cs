using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using real_time_online_chats.Server.Extensions;

namespace real_time_online_chats.Server.Attributes;

public class RequireUserId : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.TryGetUserId(out var userId))
        {
            context.HttpContext.Items["UserId"] = userId;
            return;
        }

        context.Result = new UnauthorizedResult();
    }
}