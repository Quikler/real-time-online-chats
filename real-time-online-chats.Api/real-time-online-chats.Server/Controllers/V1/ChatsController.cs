using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;
using real_time_online_chats.Server.Contracts.V1.Responses;
using real_time_online_chats.Server.Contracts.V1.Responses.Chat;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Controllers.V1;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize]
public class ChatsController(IChatService chatService, IHubContext<MessageHub, IMessageClient> messageHub) : ControllerBase
{
    private readonly IChatService _chatService = chatService;
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

        return result.Match<IActionResult>(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => BadRequest(new FailureResponse(failure.Errors))
        );
    }

    [HttpGet(ApiRoutes.Chats.GetDetailed)]
    public async Task<IActionResult> GetDetailed([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.GetChatDetailedByIdAsync(chatId);

        return result.Match<IActionResult>(
            chatDetailedDto => Ok(chatDetailedDto.ToResponse()),
            failure => failure.FailureCode switch
            {
                FailureCode.NotFound => NotFound(failure.ToResponse()),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }

    [HttpGet(ApiRoutes.Chats.GetAllOwned)]
    public async Task<IActionResult> GetAllOwned([FromQuery] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _chatService.GetAllOwnedChatsAsync(page, pageSize, userId);

        return result.Match<IActionResult>(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => BadRequest(new FailureResponse(failure.Errors))
        );
    }

    [HttpPost(ApiRoutes.Chats.Create)]
    public async Task<IActionResult> Create([FromBody] CreateChatRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        CreateChatDto createChatDto = request.ToDto();
        var result = await _chatService.CreateChatAsync(createChatDto);

        return result.Match<IActionResult>(
            chatPreviewDto => CreatedAtAction(nameof(GetDetailed), new { chatId = chatPreviewDto.Id }, chatPreviewDto.ToResponse()),
            failure => failure.FailureCode switch
            {
                FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }

    [HttpPut(ApiRoutes.Chats.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid chatId, [FromBody] UpdateChatRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.UpdateChatAsync(chatId, request.ToDto(), userId);
        
        return result.Match<IActionResult>(
            success => Ok(),
            failure => failure.FailureCode switch
            {
                FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                FailureCode.Forbidden => Forbid(),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }

    [HttpDelete(ApiRoutes.Chats.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.DeleteChatAsync(chatId, userId);
        
        return result.Match<IActionResult>(
            success => NoContent(),
            failure => failure.FailureCode switch
            {
                FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                FailureCode.Forbidden => Forbid(),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }

    [HttpPost(ApiRoutes.Chats.Join)]
    public async Task<IActionResult> Join([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.UserJoinChatAsync(chatId, userId);
        
        if (result.IsSuccess)
        {
            await _messageHub.Clients.Group(chatId.ToString()).JoinChat(userId);
        }
        
        return result.Match<IActionResult>(
            success => Ok(),
            failure => failure.FailureCode switch
            {
                FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                FailureCode.NotFound => NotFound(failure.ToResponse()),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }

    [HttpPost(ApiRoutes.Chats.Leave)]
    public async Task<IActionResult> Leave([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _chatService.UserLeaveChatAsync(chatId, userId);

        if (result.IsSuccess)
        {
            await _messageHub.Clients.Group(chatId.ToString()).LeaveChat(userId);
        }

        return result.Match<IActionResult>(
            success => Ok(),
            failure => failure.FailureCode switch
            {
                FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                FailureCode.NotFound => NotFound(failure.ToResponse()),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }
}