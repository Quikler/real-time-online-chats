using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Message;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Chat;

namespace real_time_online_chats.Server.Services.Message;

public class ChatMessageService(AppDbContext dbContext, IMessageAuthorizationService messageAuthorizationService, IChatAuthorizationService chatAuthorizationService) : IChatMessageService
{
    private readonly IMessageAuthorizationService _messageAuthorizationService = messageAuthorizationService;
    private readonly IChatAuthorizationService _chatAuthorizationService = chatAuthorizationService;
    private readonly AppDbContext _dbContext = dbContext;

    public Task<List<MessageChatDto>> GetAllMessagesByChatIdAsync(Guid chatId) => _dbContext.Messages
        .AsNoTracking()
        .Where(m => m.ChatId == chatId)
        .Include(m => m.User)
        .OrderBy(m => m.CreationTime)
        .Select(m => m.ToMessageChat())
        .ToListAsync();

    public async Task<ResultDto<MessageChatDto>> GetMessageByIdAsync(Guid chatId, Guid messageId, Guid userId)
    {
        if (!await _messageAuthorizationService.UserOwnsMessageAsync(messageId, userId)) return FailureDto.Forbidden("User doesn't own this message.");

        var message = await _dbContext.Messages
            .AsNoTracking()
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.ChatId == chatId && m.Id == messageId);

        return message is null ? FailureDto.NotFound("Message not found.") : message.ToMessageChat();
    }

    public async Task<ResultDto<MessageChatDto>> CreateMessageAsync(Guid chatId, CreateMessageDto createMessageDto)
    {
        if (!_dbContext.Chats.Any(c => c.Id == chatId)) return FailureDto.NotFound("Chat not found.");
        if (!await _chatAuthorizationService.IsUserExistInChatAsync(chatId, createMessageDto.UserId)) return FailureDto.Forbidden("User doesn't exist in chat.");

        var message = createMessageDto.ToMessage();
        message.ChatId = chatId;

        await _dbContext.Messages.AddAsync(message);
        int rows = await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(message).Reference(m => m.User).LoadAsync();

        return rows > 0 ? message.ToMessageChat() : FailureDto.BadRequest("Cannot create message.");
    }

    public async Task<ResultDto<MessageChatDto>> UpdateMessageContentAsync(Guid chatId, Guid messageId, UpdateMessageDto updateMessageDto)
    {
        if (!await _messageAuthorizationService.UserOwnsMessageAsync(messageId, updateMessageDto.UserId)) return FailureDto.Forbidden("User doesn't own this message.");

        var message = await _dbContext.Messages
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.ChatId == chatId && m.Id == messageId);

        if (message is null) return FailureDto.NotFound("Message not found.");

        message.Content = updateMessageDto.Content;
        int rows = await _dbContext.SaveChangesAsync();

        return rows > 0 ? message.ToMessageChat() : FailureDto.BadRequest("Cannot update message.");
    }

    public async Task<Result<Guid, FailureDto>> DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId)
    {
        if (!await _messageAuthorizationService.UserOwnsMessageAsync(messageId, userId)) return FailureDto.Forbidden("User doesn't own this message.");

        int rows = await _dbContext.Messages
            .Where(m => m.ChatId == chatId && m.Id == messageId)
            .ExecuteDeleteAsync();

        return rows > 0 ? messageId : FailureDto.BadRequest("Cannot delete message.");
    }
}
