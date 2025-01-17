using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Message;

public class MessageService : IMessageService
{
    private readonly AppDbContext _dbContext;

    public MessageService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<MessageEntity>> GetMessagesAsync() => _dbContext.Messages.AsNoTracking().ToListAsync();

    public async Task<MessageEntity?> GetMessageByIdAsync(Guid messageId) => await _dbContext.Messages.FindAsync(messageId);

    public async Task<bool> CreateMessageAsync(MessageEntity message)
    {
        var chat = await _dbContext.Chats.FindAsync(message.ChatId);
        if (chat is null) return false;

        await _dbContext.Messages.AddAsync(message);
        int rows = await _dbContext.SaveChangesAsync();
        return rows > 0;
    }

    public async Task<bool> UpdateMessageAsync(MessageEntity message)
    {
        var chat = await _dbContext.Chats.FindAsync(message.ChatId);
        if (chat is null) return false;

        int rows = await _dbContext.Messages
            .Where(m => m.Id == message.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Content, message.Content)
                .SetProperty(m => m.ContentType, message.Content));
        return rows > 0;
    }

    public async Task<bool> DeleteMessageAsync(Guid messageId)
    {
        int rows = await _dbContext.Messages
            .Where(m => m.Id == messageId)
            .ExecuteDeleteAsync();
        return rows > 0;
    }

    public async Task<bool> UserOwnsMessageAsync(Guid messageId, Guid userId)
    {
        var message = await _dbContext.Messages.FindAsync(messageId);
        return message is not null && message.UserId == userId;
    }
}