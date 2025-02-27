using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Controllers.V1;

[Authorize]
public class ChatsController(
    IChatService chatService,
    IHubContext<MessageHub, IMessageClient> messageHub)
    : BaseController
{
    [HttpGet(ApiRoutes.Chats.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] ChatsPaginationRequest request)
    {
        var result = await chatService.GetChatsAsync(request.PageNumber, request.PageSize);

        return result.Match(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.Chats.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid chatId, [FromQuery] ChatLevel level = ChatLevel.Preview)
    {
        switch (level)
        {
            case ChatLevel.Preview:
                var chatPreviewResult = await chatService.GetChatPreviewByIdAsync(chatId);
                return chatPreviewResult.Match(
                    chatPreviewDto => Ok(chatPreviewDto.ToResponse()),
                    failure => failure.ToActionResult()
                );

            case ChatLevel.Detailed:
                var chatDetailedResult = await chatService.GetChatDetailedByIdAsync(chatId);
                return chatDetailedResult.Match(
                    chatDetailedDto => Ok(chatDetailedDto.ToResponse()),
                    failure => failure.ToActionResult()
                );

            default:
                return BadRequest("Invalid chat level");
        }
    }

    [HttpPost(ApiRoutes.Chats.Create)]
    public async Task<IActionResult> Create([FromBody] CreateChatRequest request)
    {
        CreateChatDto createChatDto = request.ToDto(UserId);
        var result = await chatService.CreateChatAsync(createChatDto);

        return result.Match(
            chatPreviewDto => CreatedAtAction(nameof(Get), new { chatId = chatPreviewDto.Id }, chatPreviewDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpPut(ApiRoutes.Chats.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid chatId, [FromBody] UpdateChatRequest request)
    {
        var result = await chatService.UpdateChatAsync(chatId, request.ToDto(), UserId);

        return result.Match(
            success => Ok(),
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.Chats.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid chatId)
    {
        var result = await chatService.DeleteChatAsync(chatId, UserId);

        return result.Match(
            success => NoContent(),
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Chats.Join)]
    public async Task<IActionResult> Join([FromRoute] Guid chatId)
    {
        var result = await chatService.UserJoinChatAsync(chatId, UserId);

        return await result.MatchAsync(
            async (userTyple) =>
            {
                if (userTyple.isAlreadyInChat) return Ok();

                var response = userTyple.user.ToResponse();
                await messageHub.Clients.Group(chatId.ToString()).JoinChat(response);
                return Ok(response);
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Chats.Leave)]
    public async Task<IActionResult> Leave([FromRoute] Guid chatId)
    {
        var result = await chatService.UserLeaveChatAsync(chatId, UserId);

        return await result.MatchAsync<IActionResult>(
            async userChatDto =>
            {
                var response = userChatDto.ToResponse();

                await messageHub.Clients.Group(chatId.ToString()).LeaveChat(response);
                return Ok(response);
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.Chats.Kick)]
    public async Task<IActionResult> Kick([FromRoute] Guid chatId, [FromRoute] Guid memberId)
    {
        var result = await chatService.KickMemberAsync(chatId, memberId, UserId);

        return await result.MatchAsync<IActionResult>(
            async success =>
            {
                await messageHub.Clients.Group(chatId.ToString()).KickMember(memberId);
                return NoContent();
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPatch(ApiRoutes.Chats.ChangeOwner)]
    public async Task<IActionResult> ChangeOwner([FromRoute] Guid chatId, [FromQuery] Guid newOwnerId)
    {
        var result = await chatService.ChangeOwnerAsync(chatId, newOwnerId, UserId);

        return await result.MatchAsync<IActionResult>(
            async success =>
            {
                await messageHub.Clients.Group(chatId.ToString()).ChangeOwner(UserId, newOwnerId);
                return Ok();
            },
            failure => failure.ToActionResult()
        );
    }
}