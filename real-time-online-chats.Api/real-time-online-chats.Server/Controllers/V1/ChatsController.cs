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

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize]
public class ChatsController(IChatService chatService, IHubContext<MessageHub, IMessageClient> messageHub, IChatAuthorizationService chatAuthorizationService) : ControllerBase
{
    private readonly IChatService _chatService = chatService;
    private readonly IChatAuthorizationService _chatAuthorizationService = chatAuthorizationService;
    private readonly IHubContext<MessageHub, IMessageClient> _messageHub = messageHub;

    [HttpGet(ApiRoutes.Chats.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] int countOfMessages = 3)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest(new { Errors = "Page number and page size must be greater than 0." });
        }

        if (countOfMessages < 0 || countOfMessages > 4)
        {
            return BadRequest(new { Errors = "Count of messages must be greater than or equal to 0." });
        }

        var result = await _chatService.GetChatsAsync(page, pageSize);

        return result.Match(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.Chats.GetDetailed)]
    public async Task<IActionResult> GetDetailed([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.GetChatDetailedByIdAsync(chatId);

        return result.Match(
            chatDetailedDto => Ok(chatDetailedDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.Chats.GetAllOwned)]
    public async Task<IActionResult> GetAllOwned([FromQuery] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _chatService.GetAllOwnedChatsAsync(page, pageSize, userId);

        return result.Match(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Chats.Create)]
    public async Task<IActionResult> Create([FromBody] CreateChatRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        CreateChatDto createChatDto = request.ToDto(userId);
        var result = await _chatService.CreateChatAsync(createChatDto);

        return result.Match(
            chatPreviewDto => CreatedAtAction(nameof(GetDetailed), new { chatId = chatPreviewDto.Id }, chatPreviewDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpPut(ApiRoutes.Chats.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid chatId, [FromBody] UpdateChatRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.UpdateChatAsync(chatId, request.ToDto(), userId);

        return result.Match(
            success => Ok(),
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.Chats.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.DeleteChatAsync(chatId, userId);

        return result.Match(
            success => NoContent(),
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Chats.Join)]
    public async Task<IActionResult> Join([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        if (await _chatAuthorizationService.IsUserExistInChatAsync(chatId, userId)) return Ok();

        var result = await _chatService.UserJoinChatAsync(chatId, userId);

        return await result.Match<Task<IActionResult>>(
            async userChatDto =>
            {
                var response = userChatDto.ToResponse();
                await _messageHub.Clients.Group(chatId.ToString()).JoinChat(response);
                return Ok(response);
            },
            failure => Task.FromResult(failure.ToActionResult())
        );
    }

    [HttpPost(ApiRoutes.Chats.Leave)]
    public async Task<IActionResult> Leave([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.UserLeaveChatAsync(chatId, userId);

        return await result.Match<Task<IActionResult>>(
            async userChatDto =>
            {
                var response = userChatDto.ToResponse();

                await _messageHub.Clients.Group(chatId.ToString()).LeaveChat(response);
                return Ok(response);
            },
            failure => Task.FromResult(failure.ToActionResult())
        );
    }

    [HttpDelete(ApiRoutes.Chats.Kick)]
    public async Task<IActionResult> Kick([FromRoute] Guid chatId, [FromRoute] Guid memberId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.KickMemberAsync(chatId, memberId, userId);

        return await result.Match<Task<IActionResult>>(
            async success =>
            {
                await _messageHub.Clients.Group(chatId.ToString()).KickMember(memberId);
                return NoContent();
            },
            failure => Task.FromResult(failure.ToActionResult())
        );
    }
}