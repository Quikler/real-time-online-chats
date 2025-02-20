using Google.Apis.Auth;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Auth;

namespace real_time_online_chats.Server.Services.Google;

public interface IGoogleService
{
    Task<Result<AuthSuccessDto, FailureDto>> LoginAsync(GoogleJsonWebSignature.Payload payload);
    Task<Result<AuthSuccessDto, FailureDto>> SignupAsync(GoogleJsonWebSignature.Payload payload);
    Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string credential);
}