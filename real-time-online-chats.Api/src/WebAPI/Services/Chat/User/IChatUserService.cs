using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Services.Chat;

public interface IChatUserService
{
    Task<List<UserChatDto>> GetAllUsersByChatId(Guid chatId);
    Task<ResultDto<UserChatDto>> AddUser(Guid chatId, Guid userId);
    Task<ResultDto<UserChatDto>> DeleteUser(Guid chatId, Guid userId);
    Task<ResultDto<UserChatDto>> DeleteUser(Guid chatId, Guid userId, Guid whoDeletesId);
}
