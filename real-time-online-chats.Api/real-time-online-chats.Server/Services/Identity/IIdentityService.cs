using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Auth;

namespace real_time_online_chats.Server.Services.Identity;

public interface IIdentityService
{
    Task<Result<bool, FailureDto>> ConfirmEmailAsync(Guid userId, string token);
    Task<Result<EmailConfirmDto, FailureDto>> SignupAsync(SignupUserDto signupUser);
    Task<Result<AuthSuccessDto, FailureDto>> LoginAsync(LoginUserDto loginUser);
    Task<Result<AuthSuccessDto, FailureDto>> RefreshTokenAsync(string refreshToken);
    Task<Result<AuthSuccessDto, FailureDto>> MeAsync(string refreshToken);
}