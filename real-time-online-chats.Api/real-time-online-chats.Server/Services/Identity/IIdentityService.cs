using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Validation;

namespace real_time_online_chats.Server.Services.Identity;

public interface IIdentityService
{
    Task<Result<AuthResult, AuthValidationFail>> SignupAsync(SignupUser signupUser);
    Task<Result<AuthResult, AuthValidationFail>> LoginAsync(LoginUser loginUser);
    Task<Result<AuthResult, AuthValidationFail>> RefreshTokenAsync(string refreshToken);
    Task<Result<AuthResult, AuthValidationFail>> MeAsync(string refreshToken);
}