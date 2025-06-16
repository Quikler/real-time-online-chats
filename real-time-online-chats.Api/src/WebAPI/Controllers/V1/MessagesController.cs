using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Message;

namespace real_time_online_chats.Server.Controllers.V1;

[Authorize]
public class MessagesController(IMessageService messageService, IHubContext<MessageHub, IMessageClient> messagehub) : AuthorizeController
{
    [HttpGet(ApiRoutes.Messages.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid messageId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await messageService.GetMessageByIdAsync(messageId, userId);

        return result.Match(
            messageChatDto => Ok(messageChatDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Messages.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMessageRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await messageService.CreateMessageAsync(request.ToDto(userId));

        return await result.MatchAsync<IActionResult>(
            async messageChatDto =>
            {
                var response = messageChatDto.ToResponse();

                await messagehub.Clients.Group(request.ChatId.ToString()).SendMessage(response);
                return CreatedAtAction(nameof(Get), new { messageId = response.Id }, response);
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPut(ApiRoutes.Messages.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid messageId, [FromBody] UpdateMessageRequest request)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await messageService.UpdateMessageAsync(messageId, request.ToDto(userId));

        return await result.MatchAsync<IActionResult>(
            async messageChatDto =>
            {
                var response = messageChatDto.ToResponse();

                await messagehub.Clients.Group(request.ChatId.ToString()).UpdateMessage(response);
                return Ok(response);
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.Messages.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid messageId, [FromQuery] Guid chatId)
    {
        if (!HttpContext.TryGetUserId(out var userId)) return Unauthorized();

        var result = await messageService.DeleteMessageAsync(messageId, userId);

        if (result.IsSuccess)
        {
            await messagehub.Clients.Group(chatId.ToString()).DeleteMessage(messageId);
        }

        return result.Match(
            guid => NoContent(),
            failure => failure.ToActionResult()
        );
    }
}
