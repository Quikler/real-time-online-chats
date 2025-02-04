using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;
using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Services.Message;

namespace real_time_online_chats.Server.Controllers.V1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<MessageHub, IMessageClient> _messageHub;

    public MessagesController(IMessageService chatService, IHubContext<MessageHub, IMessageClient> messagehub)
    {
        _messageService = chatService;
        _messageHub = messagehub;
    }

    [HttpGet(ApiRoutes.Messages.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var messages = await _messageService.GetMessagesAsync();
        var response = messages.Select(m => new MessageChatResponse
        {
            Id = m.Id,
            Content = m.Content,
        });
        
        return Ok(response);
    }

    [HttpGet(ApiRoutes.Messages.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid messageId)
    {
        var message = await _messageService.GetMessageByIdAsync(messageId);
        if (message is null) return NotFound();
        
        var response = new MessageChatResponse
        {
            Id = message.Id,
            Content = message.Content,
        };

        return Ok(response);
    }

    [HttpPost(ApiRoutes.Messages.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMessageRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var message = new MessageEntity
        {
            UserId = userId,
            Content = request.Content,
            ContentType = request.ContentType,
            ChatId = request.ChatId,
        };

        var created = await _messageService.CreateMessageAsync(message);
        if (!created) return BadRequest(new { Message = "Failed to create message. Please try again." });
        
        var response = new MessageChatResponse
        {
            Id = message.Id,
            Content = message.Content,
        };

        await _messageHub.Clients.Group(request.ChatId.ToString()).SendMessage(response);

        return CreatedAtAction(nameof(Get), new { messageId = message.Id }, response);
    }

    [HttpPut(ApiRoutes.Messages.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid messageId, [FromBody] UpdateMessageRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var userOwnsMessage = await _messageService.UserOwnsMessageAsync(messageId, userId);
        if (!userOwnsMessage) return Forbid();

        var message = new MessageEntity
        {
            Id = messageId,
            Content = request.Content,
            ContentType = request.ContentType,
        };

        var updated = await _messageService.UpdateMessageAsync(message);

        return updated ? Ok(new MessageChatResponse
        {
            Id = message.Id,
            Content = message.Content,
            
        }) : NotFound();
    }

    [HttpDelete(ApiRoutes.Messages.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid messageId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var userOwnsMessage = await _messageService.UserOwnsMessageAsync(messageId, userId);
        if (!userOwnsMessage) return Forbid();

        var deleted = await _messageService.DeleteMessageAsync(messageId);
        return deleted ? NoContent() : NotFound();
    }
}