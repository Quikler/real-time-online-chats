
using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Repositories.Chat;

public class ChatRepository(AppDbContext dbContext) : IChatRepository
{
    public virtual async Task<int> AddChatAsync(ChatEntity chat, CancellationToken cancellationToken = default)
    {
        await dbContext.Chats.AddAsync(chat, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> DeleteChatAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Chats
            .Where(c => c.Id == chatId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    public virtual Task<bool> IsChatExistAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return dbContext.Chats.AnyAsync(c => c.Id == chatId, cancellationToken: cancellationToken);
    }

    public virtual async Task<int> UpdateChatTitleAsync(Guid chatId, string title, CancellationToken cancellationToken = default)
    {
        return await dbContext.Chats
            .Where(c => c.Id == chatId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Title, title), cancellationToken: cancellationToken);
    }
}