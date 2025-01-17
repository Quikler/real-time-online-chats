using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Chat;

public interface IChatService
{
    Task<PaginatedResult<ChatEntity>> GetChatsAsync(int pageNumber, int pageSize);
    Task<ChatEntity?> GetChatByIdAsync(Guid chatId);
    Task<ChatEntity?> GetChatRestrictByIdAsync(Guid chatId, Guid userId);
    Task<bool> CreateChatAsync(ChatEntity chat);
    Task<bool> UpdateChatAsync(ChatEntity chat);
    Task<bool> DeleteChatAsync(Guid chatId);
    Task<bool> UserExistInChatAsync(Guid chatId, Guid userId);
    Task<bool> UserJoinChatAsync(Guid chatId, UserEntity user);
}