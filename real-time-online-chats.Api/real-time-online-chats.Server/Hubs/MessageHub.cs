using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessageHub(IChatService chatService) : Hub<IMessageClient>
{
    private readonly IChatService _chatService = chatService;

    public async Task<bool> JoinChatGroup(Guid chatId)
    {
        if (Context.User is null || !Context.User.TryGetUserId(out var userId)) return false;
        //if (!await _chatService.IsUserExistInChatAsync(chatId, userId)) return false;

        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        return true;
    }
}