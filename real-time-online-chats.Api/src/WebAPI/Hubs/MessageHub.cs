using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Hubs;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessageHub(IChatAuthorizationService chatAuthorizationService) : Hub<IMessageClient>
{
    //private readonly IChatAuthorizationService _chatAuthorizationService = chatAuthorizationService;

    public async Task<bool> JoinChatGroup(Guid chatId)
    {
        //if (Context.User is null || !Context.User.TryGetUserId(out var userId)) return false;
        //if (!await _chatAuthorizationService.IsUserExistInChatAsync(chatId, userId)) return false;

        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        return true;
    }
}