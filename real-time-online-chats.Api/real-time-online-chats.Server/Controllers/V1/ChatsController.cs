using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;
using real_time_online_chats.Server.Contracts.V1.Responses;
using real_time_online_chats.Server.Contracts.V1.Responses.Auth;
using real_time_online_chats.Server.Contracts.V1.Responses.Chat;
using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Controllers.V1;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<MessageHub, IMessageClient> _messageHub;

    public ChatsController(IChatService chatService, IHubContext<MessageHub, IMessageClient> messageHub)
    {
        _chatService = chatService;
        _messageHub = messageHub;
    }

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

        var paginationChats = await _chatService.GetChatsAsync(page, pageSize);
        var response = new PaginatedResponse<GetChatResponse>
        {
            TotalCount = paginationChats.TotalCount,
            PageNumber = paginationChats.PageNumber,
            PageSize = paginationChats.PageSize,
            Items = paginationChats.Items.Select(c => new GetChatResponse
            {
                Id = c.Id,
                OwnerId = c.OwnerId,
                Title = c.Title,
            }),
        };

        return Ok(response);
    }

    [HttpGet(ApiRoutes.Chats.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid chatId)
    {
        var chat = await _chatService.GetChatByIdAsync(chatId);
        if (chat is null) return NotFound();

        var response = new GetChatResponse
        {
            Id = chat.Id,
            OwnerId = chat.OwnerId,
            Title = chat.Title,
        };

        return Ok(response);
    }

    [HttpGet(ApiRoutes.Chats.GetDetailed)]
    public async Task<IActionResult> GetDetailed([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();
        if (!await _chatService.IsUserExistInChatAsync(chatId, userId)) return Forbid();

        var chat = await _chatService.GetChatByIdWithDetailsAsync(chatId);
        if (chat is null) return NotFound();

        var response = new GetChatDetailedResponse
        {
            Id = chat.Id,
            OwnerId = chat.OwnerId,
            Title = chat.Title,
            CreationTime = chat.CreationTime,
            Messages = chat.Messages.Select(m => new GetMessageDetailedResponse
            {
                Id = m.Id,
                Content = m.Content,
                User = new UserResponse
                {
                    Id = m.UserId,
                    Email = m.User.Email,
                    FirstName = m.User.FirstName,
                    LastName = m.User.LastName,
                },
            }),
            Users = chat.Members.Select(m => new UserResponse
            {
                Id = m.Id,
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName,
            }).Append(new UserResponse
            {
                Id = chat.Owner.Id,
                Email = chat.Owner.Email,
                FirstName = chat.Owner.FirstName,
                LastName = chat.Owner.LastName,
            })
        };

        return Ok(response);
    }

    [HttpGet(ApiRoutes.Chats.GetOwned)]
    public async Task<IActionResult> GetOwned([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var paginationChats = await _chatService.GetOwnedChatsAsync(page, pageSize, userId);

        var response = new PaginatedResponse<GetChatResponse>
        {
            TotalCount = paginationChats.TotalCount,
            PageNumber = paginationChats.PageNumber,
            PageSize = paginationChats.PageSize,
            Items = paginationChats.Items.Select(c => new GetChatResponse
            {
                Id = c.Id,
                OwnerId = c.OwnerId,
                Title = c.Title,
            }),
        };

        return Ok(response);
    }

    [HttpPost(ApiRoutes.Chats.Create)]
    public async Task<IActionResult> Create([FromBody] CreateChatRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var chat = new ChatEntity
        {
            OwnerId = userId,
            Title = request.Title,
            Members = [.. request.UsersIds.Select(i => new UserEntity { Id = i })]
        };

        var created = await _chatService.CreateChatAsync(chat);
        if (!created) return BadRequest(new { Message = "Failed to create chat. Please try again." });

        var response = new CreateChatResponse
        {
            Id = chat.Id,
            OwnerId = chat.OwnerId,
            Title = chat.Title,
        };

        return CreatedAtAction(nameof(Get), new { chatId = chat.Id }, response);
    }

    [HttpPut(ApiRoutes.Chats.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid chatId, [FromBody] UpdateChatRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();
        if (!await _chatService.IsUserOwnsChatAsync(chatId, userId)) return Forbid();

        var chat = new ChatEntity
        {
            Id = chatId,
            Title = request.Title,
        };

        var updated = await _chatService.UpdateChatAsync(chat);
        var response = new GetChatResponse
        {
            Id = chat.Id,
            OwnerId = chat.OwnerId,
            Title = chat.Title,
        };

        return updated ? Ok(response) : NotFound();
    }

    [HttpDelete(ApiRoutes.Chats.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();
        if (!await _chatService.IsUserOwnsChatAsync(chatId, userId)) return Forbid();

        var deleted = await _chatService.DeleteChatAsync(chatId);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost(ApiRoutes.Chats.Join)]
    public async Task<IActionResult> Join([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();
        if (await _chatService.IsUserExistInChatAsync(chatId, userId)) return Ok();

        var joined = await _chatService.UserJoinChatAsync(chatId, userId);
        await _messageHub.Clients.Group(chatId.ToString()).JoinChat(userId);

        return Ok(joined);
    }

    [HttpPost(ApiRoutes.Chats.Leave)]
    public async Task<IActionResult> Leave([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var leaved = await _chatService.UserLeaveChatAsync(chatId, userId);
        await _messageHub.Clients.Group(chatId.ToString()).LeaveChat(userId);

        return Ok(leaved);
    }
}