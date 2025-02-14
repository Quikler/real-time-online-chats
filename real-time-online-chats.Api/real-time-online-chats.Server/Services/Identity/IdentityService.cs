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

public class IdentityService(AppDbContext dbContext,
    UserManager<UserEntity> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtConfiguration> jwtConfiguration)
    : AuthBaseService(userManager, tokenProvider, jwtConfiguration, dbContext), IIdentityService
{
    public async Task<Result<AuthSuccessDto, FailureDto>> SignupAsync(SignupUserDto signupUser)
    {
        var existingUser = await userManager.FindByEmailAsync(signupUser.Email);

        if (existingUser is not null) return FailureDto.Conflict("Email is already registered.");

        UserEntity user = signupUser.ToUser();
        user.UserName = signupUser.Email;

        var createdResult = await userManager.CreateAsync(user, signupUser.Password);

        if (!createdResult.Succeeded) return FailureDto.BadRequest(createdResult.Errors.Select(e => e.Description));
        var roles = await userManager.GetRolesAsync(user);

        return await GenerateAuthSuccessDtoForUserAsync(user);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> LoginAsync(LoginUserDto loginUser)
    {
        var user = await userManager.FindByEmailAsync(loginUser.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, loginUser.Password)) return FailureDto.Unauthorized("Invalid email or password.");
        if (await userManager.IsLockedOutAsync(user)) return FailureDto.Unauthorized("Account is locked. Please try again later.");

        await userManager.ResetAccessFailedCountAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        return await GenerateAuthSuccessDtoForUserAsync(user);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow) return FailureDto.Unauthorized("Refresh token has expired.");

        var newRefreshToken = tokenProvider.GenerateRefreshToken();
        storedRefreshToken.Token = newRefreshToken;
        storedRefreshToken.ExpiryDate = DateTime.UtcNow.Add(jwtConfiguration.RefreshTokenLifetime);

        await dbContext.SaveChangesAsync();

        var roles = await userManager.GetRolesAsync(storedRefreshToken.User);
        var token = tokenProvider.CreateToken(storedRefreshToken.User, roles);

        return CreateAuthDto(storedRefreshToken.User, newRefreshToken, token);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> MeAsync(string refreshToken)
    {
        var storedRefreshToken = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (storedRefreshToken is null) return FailureDto.Unauthorized("Refresh token has expired.");

        var roles = await userManager.GetRolesAsync(storedRefreshToken.User);
        var token = tokenProvider.CreateToken(storedRefreshToken.User, roles);

        return CreateAuthDto(storedRefreshToken.User, storedRefreshToken.Token, token);
    }
}