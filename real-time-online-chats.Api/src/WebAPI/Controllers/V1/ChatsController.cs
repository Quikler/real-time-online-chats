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
    : AuthorizeController
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
        }

        return BadRequest("Invalid chat level");
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

    [HttpPatch(ApiRoutes.Chats.UpdateTitle)]
    public async Task<IActionResult> UpdateTitle([FromRoute] Guid chatId, [FromBody] UpdateChatRequest request)
    {
        var result = await chatService.UpdateChatTitleAsync(chatId, request.ToDto(), UserId);

        return result.Match(
            success => Ok(),
            failure => failure.ToActionResult()
        );
    }

    [HttpPatch(ApiRoutes.Chats.UpdateOwner)]
    public async Task<IActionResult> UpdateOwner([FromRoute] Guid chatId, [FromBody] UpdateChatOwnerRequest request)
    {
        var result = await chatService.UpdateOwnerAsync(chatId, request.NewOwnerId, UserId);

        return await result.MatchAsync<IActionResult>(
            async success =>
            {
                await messageHub.Clients.Group(chatId.ToString()).UpdateOwner(UserId, request.NewOwnerId);
                return Ok();
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.Chats.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid chatId)
    {
        var result = await chatService.DeleteChatAsync(chatId, UserId);

        return await result.MatchAsync<IActionResult>(
            async success =>
            {
                await messageHub.Clients.Group(chatId.ToString()).DeleteChat();
                return NoContent();
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Chats.AddMemberMe)]
    public async Task<IActionResult> AddMemberMe([FromRoute] Guid chatId)
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

    [HttpDelete(ApiRoutes.Chats.DeleteMember)]
    public async Task<IActionResult> DeleteMember([FromRoute] Guid chatId, [FromRoute] Guid memberId)
    {
        var kickResult = await chatService.KickMemberAsync(chatId, memberId, UserId);

        return await kickResult.MatchAsync<IActionResult>(
            async userChatDto =>
            {
                await messageHub.Clients.Group(chatId.ToString()).KickMember(memberId);
                return NoContent();
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.Chats.DeleteMemberMe)]
    public async Task<IActionResult> DeleteMe([FromRoute] Guid chatId)
    {
        var leaveResult = await chatService.UserLeaveChatAsync(chatId, UserId);

        return await leaveResult.MatchAsync<IActionResult>(
            async userChatDto =>
            {
                await messageHub.Clients.Group(chatId.ToString()).LeaveChat(userChatDto.ToResponse());
                return NoContent();
            },
            failure => failure.ToActionResult()
        );
    }
}