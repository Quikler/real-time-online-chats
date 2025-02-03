using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Chat;

public class ChatService(AppDbContext dbContext) : IChatService
{
    private readonly AppDbContext _dbContext = dbContext;

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

    public async Task<ChatEntity?> GetChatByIdWithDetailsAsync(Guid chatId)
    {
        return await _dbContext.Chats
            .Include(c => c.Owner)
            .Include(c => c.Members)
            .Include(c => c.Messages.OrderBy(m => m.CreationTime))
                .ThenInclude(m => m.User)
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
            .SetProperty(c => c.Title, chat.Title)
        );
        return rows > 0;
    }

    public async Task<bool> DeleteChatAsync(Guid chatId)
    {
        int rows = await _dbContext.Chats.Where(c => c.Id == chatId).ExecuteDeleteAsync();
        return rows > 0;
    }

    public Task<bool> IsUserExistInChatAsync(Guid chatId, Guid userId)
    {
        return _dbContext.Chats
            .Where(c => c.Id == chatId)
            .Select(c => c.OwnerId == userId || c.Members.Any(m => m.Id == userId))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UserJoinChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is not null)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user is null) return false;

            chat.Members.Add(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> UserLeaveChatAsync(Guid chatId, Guid userId)
    {
        // Find the user and include both OwnedChats and MemberChats
        var user = await _dbContext.Users
            .Include(u => u.OwnedChats)
                .ThenInclude(oc => oc.Members)
            .Include(u => u.MemberChats)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return false;

        // Check if the user is the owner of the chat
        var ownedChat = user.OwnedChats.FirstOrDefault(c => c.Id == chatId);
        if (ownedChat is not null)
        {
            // User is the owner of the chat
            // Find another member to transfer ownership to
            var newOwner = ownedChat.Members.FirstOrDefault(u => u.Id != userId);
            if (newOwner is null)
            {
                // No other members in the chat, so delete the chat
                _dbContext.Chats.Remove(ownedChat);
            }
            else
            {
                // Transfer ownership to the new owner
                ownedChat.OwnerId = newOwner.Id;
                ownedChat.Owner = newOwner;

                // Remove the chat from the current owner's OwnedChats
                user.OwnedChats.Remove(ownedChat);

                // Add the chat to the new owner's OwnedChats
                newOwner.OwnedChats.Add(ownedChat);
            }
        }
        else
        {
            // User is a member of the chat
            var memberChat = user.MemberChats.FirstOrDefault(c => c.Id == chatId);
            if (memberChat is null) return false; // User is not a member of the chat

            // Remove the chat from the user's MemberChats
            user.MemberChats.Remove(memberChat);
        }

        // Save changes to the database
        int rows = await _dbContext.SaveChangesAsync();
        return rows > 0;
    }

    public async Task<bool> IsUserOwnsChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _dbContext.Chats.FindAsync(chatId);
        return chat is not null && chat.OwnerId == userId;
    }

    public async Task<PaginatedResult<ChatEntity>> GetOwnedChatsAsync(int pageNumber, int pageSize, Guid userId)
    {
        var totalRecords = await _dbContext.Chats.CountAsync();

        var chats = await _dbContext.Chats
            .Where(c => c.OwnerId == userId)
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

    public Task<PaginatedResult<ChatEntity>> GetOwnedChatsWithDetailsAsync(int pageNumber, int pageSize, Guid userId)
    {
        throw new NotImplementedException();
    }
}