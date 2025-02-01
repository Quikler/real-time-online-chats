using Google.Apis.Auth;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Google;

public interface IGoogleService
{
    Task<AuthResult> LoginAsync(GoogleJsonWebSignature.Payload payload);
    Task<AuthResult> SignupAsync(GoogleJsonWebSignature.Payload payload);
    Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string credential);
}