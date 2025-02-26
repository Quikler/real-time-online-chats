using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Repositories.Chat;

public interface IChatRepository
{
    Task<int> AddChatAsync(ChatEntity chat, CancellationToken cancellationToken = default);
    Task<int> UpdateChatTitleAsync(Guid chatId, string title, CancellationToken cancellationToken = default);
    Task<int> DeleteChatAsync(Guid chatId, CancellationToken cancellationToken = default);
}