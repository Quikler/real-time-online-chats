using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Extensions;

namespace real_time_online_chats.Server.Controllers.V1;

public class AuthorizeController : ControllerBase
{
    protected Guid UserId => HttpContext.GetUserId() ?? throw new Exception("User is not authenticated.");
}
