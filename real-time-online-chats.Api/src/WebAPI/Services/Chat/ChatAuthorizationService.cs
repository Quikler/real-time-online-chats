
using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;

namespace real_time_online_chats.Server.Services.Chat;

public class ChatAuthorizationService(AppDbContext dbContext) : IChatAuthorizationService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<bool> IsUserOwnsChatAsync(Guid chatId, Guid userId)
    {
        return await _dbContext.Chats
            .Where(c => c.Id == chatId)
            .Select(c => c.OwnerId)
            .FirstOrDefaultAsync() == userId;
    }

    public Task<bool> IsUserExistInChatAsync(Guid chatId, Guid userId)
    {
        return _dbContext.Chats
            .Where(c => c.Id == chatId)
            .Select(c => c.OwnerId == userId || c.Members.Any(m => m.Id == userId))
            .FirstOrDefaultAsync();
    }
}