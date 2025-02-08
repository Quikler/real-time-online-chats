using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Message;
using real_time_online_chats.Server.Mapping;

namespace real_time_online_chats.Server.Services.Message;

public class MessageService(AppDbContext dbContext, IMessageAuthorizationService messageAuthorizationService) : IMessageService
{
    private readonly IMessageAuthorizationService _messageAuthorizationService = messageAuthorizationService;
    private readonly AppDbContext _dbContext = dbContext;

    //public Task<List<MessageEntity>> GetMessagesAsync() => _dbContext.Messages.AsNoTracking().ToListAsync();

    public async Task<Result<MessageChatDto, FailureDto>> GetMessageByIdAsync(Guid messageId, Guid userId)
    {
        if (!await _messageAuthorizationService.UserOwnsMessageAsync(messageId, userId)) return FailureDto.Forbidden("User doesn't own this message.");

        var message = await _dbContext.Messages.FindAsync(messageId);

        return message is null ? FailureDto.NotFound("Message not found.") : message.ToMessageChat();
    }

    public async Task<Result<MessageChatDto, FailureDto>> CreateMessageAsync(CreateMessageDto createMessageDto)
    {
        if (!_dbContext.Chats.Any(c => c.Id == createMessageDto.ChatId)) return FailureDto.NotFound("Chat not found.");

        var message = createMessageDto.ToMessage();

        await _dbContext.Messages.AddAsync(message);
        int rows = await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(message).Reference(m => m.User).LoadAsync();

        return rows > 0 ? message.ToMessageChat() : FailureDto.BadRequest("Cannot create message.");
    }

    public async Task<Result<MessageChatDto, FailureDto>> UpdateMessageAsync(Guid messageId, UpdateMessageDto updateMessageDto)
    {
        if (!await _messageAuthorizationService.UserOwnsMessageAsync(messageId, updateMessageDto.UserId)) return FailureDto.Forbidden("User doesn't own this message.");

        var message = await _dbContext.Messages
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message is null) return FailureDto.NotFound("Message not found.");

        message.Content = updateMessageDto.Content;
        int rows = await _dbContext.SaveChangesAsync();

        return rows > 0 ? message.ToMessageChat() : FailureDto.BadRequest("Cannot update message.");
    }

    public async Task<Result<Guid, FailureDto>> DeleteMessageAsync(Guid messageId, Guid userId)
    {
        if (!await _messageAuthorizationService.UserOwnsMessageAsync(messageId, userId)) return FailureDto.Forbidden("User doesn't own this message.");

        int rows = await _dbContext.Messages
            .Where(m => m.Id == messageId)
            .ExecuteDeleteAsync();

        return rows > 0 ? messageId : FailureDto.BadRequest("Cannot delete message.");
    }
}