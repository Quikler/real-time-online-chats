using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Helpers;
using real_time_online_chats.Server.Providers;
using real_time_online_chats.Server.Validation;

namespace real_time_online_chats.Server.Services.Identity;

public class IdentityService(UserManager<UserEntity> userManager, IOptions<JwtConfiguration> jwtConfiguration,
    AppDbContext dbContext, TokenProvider tokenProvider) : IIdentityService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly TokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<AuthResult, AuthValidationFail>> SignupAsync(SignupUser signupUser)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(signupUser.Email);

            if (existingUser is not null) return new AuthValidationFail("Email is already registered.");

            var newUser = new UserEntity
            {
                Email = signupUser.Email,
                UserName = signupUser.Email,
            };

            var createdResult = await _userManager.CreateAsync(newUser, signupUser.Password);

            if (!createdResult.Succeeded)
            {
                return new AuthValidationFail(createdResult.Errors.Select(e => e.Description));
            }

            return await AuthHelper.GenerateAuthResultForUserAsync(newUser, _tokenProvider, _dbContext, _jwtConfiguration.TokenLifetime);
        }
        catch
        {
            return new AuthValidationFail("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<Result<AuthResult, AuthValidationFail>> LoginAsync(LoginUser loginUser)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginUser.Password)) return new AuthValidationFail("Invalid email or password.");
            if (await _userManager.IsLockedOutAsync(user)) return new AuthValidationFail("Account is locked. Please try again later.");

            await _userManager.ResetAccessFailedCountAsync(user);
            return await AuthHelper.GenerateAuthResultForUserAsync(user, _tokenProvider, _dbContext, _jwtConfiguration.TokenLifetime);
        }
        catch
        {
            return new AuthValidationFail("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<Result<AuthResult, AuthValidationFail>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var storedRefreshToken = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow) return new AuthValidationFail("Refresh token has expired.");

            var newRefreshToken = _tokenProvider.GenerateRefreshToken();
            storedRefreshToken.Token = newRefreshToken;
            storedRefreshToken.ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime);

            await _dbContext.SaveChangesAsync();

            return AuthHelper.CreateAuthResult(storedRefreshToken.User, refreshToken, _tokenProvider);
        }
        catch
        {
            return new AuthValidationFail("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<Result<AuthResult, AuthValidationFail>> MeAsync(string refreshToken)
    {
        var rToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (rToken is null) return new AuthValidationFail("Refresh token is not found. Token has expired");

        return AuthHelper.CreateAuthResult(rToken.User, refreshToken, _tokenProvider);
    }
}