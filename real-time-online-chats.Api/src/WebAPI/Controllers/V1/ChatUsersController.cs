using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Attributes;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Controllers.V1;

[Authorize]
public class ChatUsersController(IChatUserService chatUserService, IHubContext<MessageHub, IMessageClient> messageHub) : AuthorizeController
{
    [HttpGet(ApiRoutes.ChatUsers.GetAll)]
    [Cached(600)]
    public async Task<IActionResult> GetAll([FromRoute] Guid chatId)
    {
        var result = await chatUserService.GetAllUsersByChatId(chatId);
        return Ok(result.Select(u => u.ToResponse()));
    }

    [HttpPost(ApiRoutes.ChatUsers.AddUserMe)]
    [RemoveCache(ApiRoutes.ChatUsers.GetAll)]
    public async Task<IActionResult> AddMe([FromRoute] Guid chatId)
    {
        var result = await chatUserService.AddUser(chatId, UserId);

        return await result.MatchAsync<IActionResult>(
            async (dto) =>
            {
                var response = dto.ToResponse();
                await messageHub.Clients.Group(chatId.ToString()).JoinChat(response);
                return Ok(response);
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.ChatUsers.DeleteUserMe)]
    [RemoveCache(ApiRoutes.ChatUsers.GetAll)]
    public async Task<IActionResult> DeleteMe([FromRoute] Guid chatId)
    {
        var kickResult = await chatUserService.DeleteUser(chatId, UserId);

        return await kickResult.MatchAsync<IActionResult>(
            async dto =>
            {
                var response = dto.ToResponse();
                await messageHub.Clients.Group(chatId.ToString()).LeaveChat(response);
                return NoContent();
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.ChatUsers.DeleteUser)]
    [RemoveCache(ApiRoutes.ChatUsers.GetAll)]
    public async Task<IActionResult> DeleteMember([FromRoute] Guid chatId, [FromRoute] Guid userId)
    {
        var kickResult = await chatUserService.DeleteUser(chatId, userId, UserId);

        return await kickResult.MatchAsync<IActionResult>(
            async dto =>
            {
                await messageHub.Clients.Group(chatId.ToString()).KickMember(userId);
                return NoContent();
            },
            failure => failure.ToActionResult()
        );
    }
}
