using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Services.Google;

public class GoogleService(
    IOptions<GoogleConfiguration> googleConfiguration, 
    AppDbContext dbContext, 
    UserManager<UserEntity> userManager, 
    TokenProvider tokenProvider, 
    IOptions<JwtConfiguration> jwtConfiguration
) : IGoogleService
{
    private readonly GoogleConfiguration _googleConfiguration = googleConfiguration.Value;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public async Task<AuthResult> LoginAsync(GoogleJsonWebSignature.Payload payload)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user is null)
            {
                return new AuthResult
                {
                    Errors = ["Invalid email or password."],
                };
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new AuthResult
                {
                    Errors = ["Account is locked. Please try again later."]
                };
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            return await GenerateAuthResultForUserAsync(user);
        }
        catch
        {
            return new AuthResult
            {
                Errors = ["An unexpected error occurred. Please try again later."]
            };
        }
    }

    public async Task<AuthResult> SignupAsync(GoogleJsonWebSignature.Payload payload)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(payload.Email);
            if (existingUser is not null)
            {
                return new AuthResult
                {
                    Errors = ["Email is already registered."]
                };
            }

            var newUser = new UserEntity
            {
                Email = payload.Email,
                UserName = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
            };

            var createdUserResult = await _userManager.CreateAsync(newUser);

            if (!createdUserResult.Succeeded)
            {
                return new AuthResult
                {
                    Errors = createdUserResult.Errors.Select(e => e.Description),
                };
            }

            return await GenerateAuthResultForUserAsync(newUser);
        }
        catch
        {
            return new AuthResult
            {
                Errors = ["An unexpected error occurred. Please try again later."]
            };
        }
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

    private async Task<AuthResult> GenerateAuthResultForUserAsync(UserEntity user)
    {
        var token = _tokenProvider.CreateJwtSecurityToken(user);

        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = _tokenProvider.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime),
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new AuthResult
        {
            Succeded = true,
            Token = token,
            RefreshToken = refreshToken.Token,
            User = new UserResult
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
            }
        };
    }
}