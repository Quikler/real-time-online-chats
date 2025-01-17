using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Identity;

public interface IIdentityService
{
    Task<AuthResult> SignupAsync(SignupUser signupUser);
    Task<AuthResult> LoginAsync(LoginUser loginUser);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<AuthResult> MeAsync(string refreshToken);
}