
using real_time_online_chats.Server.Data;

namespace real_time_online_chats.Server.Services.Message;

public class MessageAuthorizationService(AppDbContext dbContext) : IMessageAuthorizationService
{
    private readonly AppDbContext _dbContext = dbContext;
    public virtual async Task<bool> UserOwnsMessageAsync(Guid messageId, Guid userId)
    {
        var message = await _dbContext.Messages.FindAsync(messageId);
        return message is not null && message.UserId == userId;
    }
}