using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;
using real_time_online_chats.Server.Contracts.V1.Responses;
using real_time_online_chats.Server.Contracts.V1.Responses.Chat;
using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Controllers.V1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet(ApiRoutes.Chats.GetAll)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 5)
    {
        if (page <= 0 || pageSize <= 0)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var paginationChats = await _chatService.GetChatsAsync(page, pageSize);
        var response = new PaginatedChatResponse
        {
            TotalCount = paginationChats.TotalCount,
            PageNumber = paginationChats.PageNumber,
            PageSize = paginationChats.PageSize,
            Chats = paginationChats.Items.Select(c => new GetChatResponse
            {
                Id = c.Id,
                OwnerId = c.OwnerId,
                Title = c.Title,
            }).ToList(),
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

    [HttpGet(ApiRoutes.Chats.GetRestrict)]
    public async Task<IActionResult> GetRestrict([FromRoute] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var chat = await _chatService.GetChatRestrictByIdAsync(chatId, userId);
        if (chat is null) return NotFound();
        
        var response = new GetChatRestrictResponse
        {
            Id = chat.Id,
            OwnerId = chat.OwnerId,
            Title = chat.Title,
            Members = chat.Members,
            Messages = chat.Messages.Select(m => new GetMessageResponse
            {
                Id = m.Id,
                Content = m.Content,
                UserId = m.UserId,
            }).ToList(),
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
        };

        var created = await _chatService.CreateChatAsync(chat);
        if (!created) return BadRequest(new { Message = "Failed to create chat. Please try again." });
        
        var response = new CreateChatResponse
        {
            OwnerId = chat.OwnerId,
            Title = chat.Title,
        };

        return CreatedAtAction(nameof(Get), new { chatId = chat.Id }, response);
    }

    [HttpPut(ApiRoutes.Chats.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid chatId, [FromBody] UpdateChatRequest request)
    {
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
        var deleted = await _chatService.DeleteChatAsync(chatId);
        return deleted ? NoContent() : NotFound();
    }
}