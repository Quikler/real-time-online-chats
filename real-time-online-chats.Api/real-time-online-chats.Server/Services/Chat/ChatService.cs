using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Chat;

public class ChatService : IChatService
{
    private readonly AppDbContext _dbContext;

    public ChatService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResult<ChatEntity>> GetChatsAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _dbContext.Chats.CountAsync();
        var chats = await _dbContext.Chats
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<ChatEntity>
        {
            Items = chats,
            TotalCount = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ChatEntity?> GetChatByIdAsync(Guid chatId) => await _dbContext.Chats.FindAsync(chatId);

    public async Task<ChatEntity?> GetChatRestrictByIdAsync(Guid chatId, Guid userId)
    {
        var isMember = await UserExistInChatAsync(chatId, userId);
        if (!isMember) return null;

        return await _dbContext.Chats
            .AsNoTracking()
            .Include(c => c.Members)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    public async Task<bool> CreateChatAsync(ChatEntity chat)
    {
        await _dbContext.Chats.AddAsync(chat);
        int rows = await _dbContext.SaveChangesAsync();
        return rows > 0;
    }

    public async Task<bool> UpdateChatAsync(ChatEntity chat)
    {
        int rows = await _dbContext.Chats.Where(c => c.Id == chat.Id).ExecuteUpdateAsync(s => s
            .SetProperty(c => c.OwnerId, chat.OwnerId)
            .SetProperty(c => c.Title, chat.Title)
        );
        return rows > 0;
    }

    public async Task<bool> DeleteChatAsync(Guid chatId)
    {
        int rows = await _dbContext.Chats.Where(c => c.Id == chatId).ExecuteDeleteAsync();
        return rows > 0;
    }

    public Task<bool> UserExistInChatAsync(Guid chatId, Guid userId)
    {
        return _dbContext.Chats
            .Where(c => c.Id == chatId)
            .Select(c => c.Members.Any(m => m.Id == userId))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UserJoinChatAsync(Guid chatId, UserEntity user)
    {
        var chat = await _dbContext.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync();

        if (chat is not null && !chat.Members.Exists(u => u.Id == user.Id))
        {
            chat.Members.Add(user);
            return true;
        }

        return false;
    }
}