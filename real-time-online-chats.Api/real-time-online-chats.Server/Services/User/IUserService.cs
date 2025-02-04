using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Services.User;

public interface IUserService
{
    Task<Result<UserProfileDto, UserFailureDto>> GetUserProfileAsync(Guid userId);
}