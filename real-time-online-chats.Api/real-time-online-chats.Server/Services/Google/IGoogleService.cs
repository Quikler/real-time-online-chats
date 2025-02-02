using Google.Apis.Auth;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Validation;

namespace real_time_online_chats.Server.Services.Google;

public interface IGoogleService
{
    Task<Result<AuthResult, AuthValidationFail>> LoginAsync(GoogleJsonWebSignature.Payload payload);
    Task<Result<AuthResult, AuthValidationFail>> SignupAsync(GoogleJsonWebSignature.Payload payload);
    Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string credential);
}