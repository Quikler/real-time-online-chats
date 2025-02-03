using System.Diagnostics.Eventing.Reader;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Chat;

public interface IChatService
{
    Task<PaginatedResult<ChatEntity>> GetChatsAsync(int pageNumber, int pageSize);
    Task<ChatEntity?> GetChatByIdAsync(Guid chatId);
    Task<ChatEntity?> GetChatByIdWithDetailsAsync(Guid chatId);
    Task<PaginatedResult<ChatEntity>> GetOwnedChatsAsync(int pageNumber, int pageSize, Guid userId);
    Task<PaginatedResult<ChatEntity>> GetOwnedChatsWithDetailsAsync(int pageNumber, int pageSize, Guid userId);
    Task<bool> CreateChatAsync(ChatEntity chat);
    Task<bool> UpdateChatAsync(ChatEntity chat);
    Task<bool> DeleteChatAsync(Guid chatId);
    Task<bool> IsUserExistInChatAsync(Guid chatId, Guid userId);
    Task<bool> IsUserOwnsChatAsync(Guid chatId, Guid userId);
    Task<bool> UserJoinChatAsync(Guid chatId, Guid userId);
    Task<bool> UserLeaveChatAsync(Guid chatId, Guid userId);
}