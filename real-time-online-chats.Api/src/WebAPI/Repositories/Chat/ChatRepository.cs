
using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;

namespace real_time_online_chats.Server.Repositories.Chat;

public class ChatRepository(AppDbContext dbContext) : IChatRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public virtual async Task<int> DeleteChatAsync(Guid chatId)
    {
        return await _dbContext.Chats
            .Where(c => c.Id == chatId)
            .ExecuteDeleteAsync();
    }

    public virtual async Task<int> UpdateChatTitleAsync(Guid chatId, string title)
    {
        return await _dbContext.Chats
            .Where(c => c.Id == chatId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Title, title));
    }
}