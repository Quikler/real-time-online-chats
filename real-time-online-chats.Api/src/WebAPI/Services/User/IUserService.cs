using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Services.User;

public interface IUserService
{
    Task<Result<UserGlobalDto, FailureDto>> GetUserGlobalAsync(Guid userId);
    Task<Result<UserProfileDto, FailureDto>> GetUserProfileAsync(Guid userId);
    Task<Result<UserProfileDto, FailureDto>> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updateUserProfileDto);
    Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetUserOwnerChatsAsync(int pageNumber, int pageSize, Guid userId);
    Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetUserMemberChatsAsync(int pageNumber, int pageSize, Guid userId);
}