using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Repositories.User;

public interface IUserRepository
{
    Task<UserChatDto?> GetUserChatDto(Guid userId);
    Task<UserFriendDto?> GetUserFriendDto(Guid userId);
    Task<UserGlobalDto?> GetUserGlobalDto(Guid userId);
    Task<UserProfileDto?> GetUserProfileDto(Guid userId);
}