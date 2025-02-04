using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.Helpers;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Services.Identity;

public class IdentityService(UserManager<UserEntity> userManager, IOptions<JwtConfiguration> jwtConfiguration,
    AppDbContext dbContext, TokenProvider tokenProvider) : IIdentityService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly TokenProvider _tokenProvider = tokenProvider;

    public async Task<Result<AuthSuccessDto, FailureDto>> SignupAsync(SignupUserDto signupUser)
    {
        var existingUser = await _userManager.FindByEmailAsync(signupUser.Email);

        if (existingUser is not null) return FailureDto.Conflict("Email is already registered.");

        UserEntity user = signupUser.ToUser();

        var createdResult = await _userManager.CreateAsync(user, signupUser.Password);

        if (!createdResult.Succeeded) return FailureDto.BadRequest(createdResult.Errors.Select(e => e.Description));

        return await AuthHelper.GenerateAuthResultForUserAsync(user, _tokenProvider, _dbContext, _jwtConfiguration.RefreshTokenLifetime);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> LoginAsync(LoginUserDto loginUser)
    {
        var user = await _userManager.FindByEmailAsync(loginUser.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginUser.Password)) return FailureDto.Unauthorized("Invalid email or password.");
        if (await _userManager.IsLockedOutAsync(user)) return FailureDto.Unauthorized("Account is locked. Please try again later.");

        await _userManager.ResetAccessFailedCountAsync(user);
        return await AuthHelper.GenerateAuthResultForUserAsync(user, _tokenProvider, _dbContext, _jwtConfiguration.RefreshTokenLifetime);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow) return FailureDto.Unauthorized("Refresh token has expired.");

        var newRefreshToken = _tokenProvider.GenerateRefreshToken();
        storedRefreshToken.Token = newRefreshToken;
        storedRefreshToken.ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime);

        await _dbContext.SaveChangesAsync();

        return AuthHelper.CreateAuthSuccess(storedRefreshToken.User, newRefreshToken, _tokenProvider);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> MeAsync(string refreshToken)
    {
        var rToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (rToken is null) return FailureDto.Unauthorized("Refresh token has expired.");

        return AuthHelper.CreateAuthSuccess(rToken.User, rToken.Token, _tokenProvider);
    }
}