using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using real_time_online_chats.Server.Attributes;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Hubs.Clients;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Message;

namespace real_time_online_chats.Server.Controllers.V1;

[Authorize]
public class ChatMessagesController(IChatMessageService messageService, IHubContext<MessageHub, IMessageClient> messagehub) : AuthorizeController
{
    [HttpGet(ApiRoutes.ChatMessages.Get)]
    [Cached(60)]
    public async Task<IActionResult> Get([FromRoute] Guid chatId, [FromRoute] Guid messageId)
    {
        var result = await messageService.GetMessageByIdAsync(chatId, messageId, UserId);

        return result.Match(
            messageChatDto => Ok(messageChatDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.ChatMessages.GetAll)]
    [Cached(60)]
    public async Task<IActionResult> GetAll([FromRoute] Guid chatId)
    {
        var result = await messageService.GetAllMessagesByChatIdAsync(chatId);
        return Ok(result.Select(m => m.ToResponse()));
    }

    [HttpPost(ApiRoutes.ChatMessages.Create)]
    [InvalidateCache(ApiRoutes.ChatMessages.Get, 60)]
    [RemoveCache(ApiRoutes.ChatMessages.GetAll)]
    public async Task<IActionResult> Create([FromRoute] Guid chatId, [FromBody] CreateMessageRequest request)
    {
        var result = await messageService.CreateMessageAsync(chatId, request.ToDto(UserId));

        return await result.MatchAsync<IActionResult>(
            async messageChatDto =>
            {
                var response = messageChatDto.ToResponse();

                await messagehub.Clients.Group(chatId.ToString()).SendMessage(response);
                return CreatedAtAction(nameof(Get), new { chatId = chatId, messageId = response.Id }, response);
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPut(ApiRoutes.ChatMessages.Update)]
    [InvalidateCache(ApiRoutes.ChatMessages.Get, 60)]
    [RemoveCache(ApiRoutes.ChatMessages.GetAll)]
    public async Task<IActionResult> Update([FromRoute] Guid chatId, [FromRoute] Guid messageId, [FromBody] UpdateMessageRequest request)
    {
        var result = await messageService.UpdateMessageContentAsync(chatId, messageId, request.ToDto(UserId));

        return await result.MatchAsync<IActionResult>(
            async messageChatDto =>
            {
                var response = messageChatDto.ToResponse();

                await messagehub.Clients.Group(chatId.ToString()).UpdateMessage(response);
                return Ok(response);
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpDelete(ApiRoutes.ChatMessages.Delete)]
    [InvalidateCache(ApiRoutes.ChatMessages.Get, 60)]
    [RemoveCache(ApiRoutes.ChatMessages.GetAll)]
    public async Task<IActionResult> Delete([FromRoute] Guid chatId, [FromRoute] Guid messageId)
    {
        var result = await messageService.DeleteMessageAsync(chatId, messageId, UserId);

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
