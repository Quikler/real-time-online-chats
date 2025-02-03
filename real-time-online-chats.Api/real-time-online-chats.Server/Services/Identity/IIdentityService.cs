using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Validation;

namespace real_time_online_chats.Server.Services.Identity;

public interface IIdentityService
{
    Task<Result<AuthSuccess, AuthFailure>> SignupAsync(SignupUser signupUser);
    Task<Result<AuthSuccess, AuthFailure>> LoginAsync(LoginUser loginUser);
    Task<Result<AuthSuccess, AuthFailure>> RefreshTokenAsync(string refreshToken);
    Task<Result<AuthSuccess, AuthFailure>> MeAsync(string refreshToken);
}