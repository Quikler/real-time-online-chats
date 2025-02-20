namespace real_time_online_chats.Server.Services.Chat;

public interface IChatAuthorizationService
{
    Task<bool> IsUserExistInChatAsync(Guid chatId, Guid userId);
    Task<bool> IsUserOwnsChatAsync(Guid chatId, Guid userId);
}