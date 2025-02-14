using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.Helpers;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Services.Google;

public class GoogleService(
    IOptions<GoogleConfiguration> googleConfiguration,
    AppDbContext dbContext,
    UserManager<UserEntity> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtConfiguration> jwtConfiguration) 
    : IGoogleService
{
    private readonly GoogleConfiguration _googleConfiguration = googleConfiguration.Value;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public async Task<Result<AuthSuccessDto, FailureDto>> LoginAsync(GoogleJsonWebSignature.Payload payload)
    {
        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user is null) return FailureDto.Unauthorized("Invalid email or password.");
        if (await _userManager.IsLockedOutAsync(user)) return FailureDto.Unauthorized("Account is locked. Please try again later.");

        await _userManager.ResetAccessFailedCountAsync(user);
        return await AuthHelper.GenerateAuthResultForUserAsync(user, _tokenProvider, _dbContext, _jwtConfiguration.RefreshTokenLifetime);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> SignupAsync(GoogleJsonWebSignature.Payload payload)
    {
        var existingUser = await _userManager.FindByEmailAsync(payload.Email);

        if (existingUser is not null) return FailureDto.Conflict("Email is already registered.");

        var newUser = new UserEntity
        {
            Email = payload.Email,
            UserName = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            AvatarUrl = payload.Picture,
        };

        var createdResult = await _userManager.CreateAsync(newUser);
        if (!createdResult.Succeeded) return FailureDto.BadRequest(createdResult.Errors.Select(e => e.Description));

        return await AuthHelper.GenerateAuthResultForUserAsync(newUser, _tokenProvider, _dbContext, _jwtConfiguration.RefreshTokenLifetime);
    }

    public async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string credential)
    {
        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_googleConfiguration.ClientId],
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(credential, validationSettings);
            return payload;
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}