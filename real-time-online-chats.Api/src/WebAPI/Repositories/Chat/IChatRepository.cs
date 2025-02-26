namespace real_time_online_chats.Server.Repositories.Chat;

public interface IChatRepository
{
    Task<int> UpdateChatTitleAsync(Guid chatId, string title);
    Task<int> DeleteChatAsync(Guid chatId);
}