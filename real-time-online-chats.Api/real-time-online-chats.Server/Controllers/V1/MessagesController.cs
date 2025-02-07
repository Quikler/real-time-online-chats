using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;
using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Message;
using real_time_online_chats.Server.Services.User;

namespace real_time_online_chats.Server.Controllers.V1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessagesController(IMessageService chatService, IHubContext<MessageHub, IMessageClient> messagehub, IUserService userService) : ControllerBase
{
    private readonly IMessageService _messageService = chatService;
    private readonly IUserService _userService = userService;
    private readonly IHubContext<MessageHub, IMessageClient> _messageHub = messagehub;

    // [HttpGet(ApiRoutes.Messages.GetAll)]
    // public async Task<IActionResult> GetAll()
    // {
    //     var messages = await _messageService.GetMessagesAsync();
    //     var response = messages.Select(m => new MessageChatResponse
    //     {
    //         Id = m.Id,
    //         Content = m.Content,
    //     });

    //     return Ok(response);
    // }

    [HttpGet(ApiRoutes.Messages.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid messageId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _messageService.GetMessageByIdAsync(messageId, userId);
        
        return result.Match<IActionResult>(
            messageChatDto => Ok(messageChatDto.ToResponse()),
            failure => failure.FailureCode switch
            {
                FailureCode.Forbidden => Forbid(),
                FailureCode.NotFound => NotFound(failure.ToResponse()),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }

    [HttpPost(ApiRoutes.Messages.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMessageRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _messageService.CreateMessageAsync(request.ToDto(userId));

        return await result.Match<Task<IActionResult>>(
            async messageChatDto =>
            {
                var response = messageChatDto.ToResponse();

                await _messageHub.Clients.Group(request.ChatId.ToString()).SendMessage(response);
                return CreatedAtAction(nameof(Get), new { messageId = response.Id }, response);
            },
            failure => 
            {
                IActionResult failureResponse = failure.FailureCode switch
                {
                    FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                    FailureCode.NotFound => NotFound(failure.ToResponse()),
                    _ => StatusCode(StatusCodes.Status500InternalServerError),
                };

                return Task.FromResult(failureResponse);
            }
        );
    }

    [HttpPut(ApiRoutes.Messages.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid messageId, [FromBody] UpdateMessageRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _messageService.UpdateMessageAsync(messageId, request.ToDto(userId));

        return await result.Match<Task<IActionResult>>(
            async messageChatDto =>
            {
                var response = messageChatDto.ToResponse();

                await _messageHub.Clients.Group(request.ChatId.ToString()).UpdateMessage(response);
                return Ok(response);
            },
            failure => 
            {
                IActionResult failureResponse = failure.FailureCode switch
                {
                    FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                    FailureCode.Forbidden => Forbid(),
                    FailureCode.NotFound => NotFound(failure.ToResponse()),
                    _ => StatusCode(StatusCodes.Status500InternalServerError),
                };

                return Task.FromResult(failureResponse);
            }
        );
    }

    [HttpDelete(ApiRoutes.Messages.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid messageId, [FromQuery] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await _messageService.DeleteMessageAsync(messageId, userId);
        
        if (result.IsSuccess)
        {
            await _messageHub.Clients.Group(chatId.ToString()).DeleteMessage(messageId);
        }

        return result.Match<IActionResult>(
            guid => NoContent(),
            failure => failure.FailureCode switch
            {
                FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                FailureCode.Forbidden => Forbid(),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }
}