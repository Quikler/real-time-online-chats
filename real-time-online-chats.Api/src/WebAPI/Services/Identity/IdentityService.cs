using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Services.Identity;

public class IdentityService(AppDbContext dbContext,
    UserManager<UserEntity> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtConfiguration> jwtConfiguration)
    : AuthBaseService(userManager, tokenProvider, jwtConfiguration, dbContext), IIdentityService
{
    public async Task<ResultDto<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user is null)
        {
            return FailureDto.NotFound("User not found");
        }

        var resetPasswordResult = await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        if (resetPasswordResult.Succeeded)
        {
            return true;
        } 

        return FailureDto.BadRequest(resetPasswordResult.Errors.Select(e => e.Description));
    }

    public async Task<ResultDto<string>> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            return resetToken;
        }

        return FailureDto.NotFound("User with given email was not found");
    }

    public async Task<Result<bool, FailureDto>> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return FailureDto.NotFound("User not found.");

        var confirmResult = await userManager.ConfirmEmailAsync(user, token);
        return confirmResult.Succeeded ? true : FailureDto.BadRequest(confirmResult.Errors.Select(e => e.Description));
    }

    public async Task<Result<EmailConfirmDto, FailureDto>> SignupAsync(SignupUserDto signupUser)
    {
        var existingUser = await userManager.FindByEmailAsync(signupUser.Email);

        if (existingUser is not null) return FailureDto.Conflict("Email is already registered.");

        UserEntity user = signupUser.ToUser();
        user.UserName = signupUser.Email;

        IdentityResult createdResult = await userManager.CreateAsync(user, signupUser.Password);
        if (!createdResult.Succeeded) return FailureDto.BadRequest(createdResult.Errors.Select(e => e.Description));

        string emailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return new EmailConfirmDto
        {
            UserId = user.Id,
            Token = emailToken,
        };
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> LoginAsync(LoginUserDto loginUser)
    {
        var user = await userManager.FindByEmailAsync(loginUser.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, loginUser.Password)) return FailureDto.Unauthorized("Invalid email or password.");
        if (!await userManager.IsEmailConfirmedAsync(user)) return FailureDto.Unauthorized("Email is not confirmed.");
        if (await userManager.IsLockedOutAsync(user)) return FailureDto.Unauthorized("Account is locked. Please try again later.");

        await userManager.ResetAccessFailedCountAsync(user);
        return await GenerateAuthSuccessDtoForUserAsync(user);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (!IsRefreshTokenValid(storedRefreshToken)) return FailureDto.Unauthorized("Refresh token has expired.");

        var newRefreshToken = tokenProvider.GenerateRefreshToken();
        storedRefreshToken!.Token = newRefreshToken;
        storedRefreshToken.ExpiryDate = DateTime.UtcNow.Add(jwtConfiguration.RefreshTokenLifetime);

        await dbContext.SaveChangesAsync();

        var roles = await userManager.GetRolesAsync(storedRefreshToken.User);
        var token = tokenProvider.CreateToken(storedRefreshToken.User, roles);

        return CreateAuthSuccessDto(storedRefreshToken.User, newRefreshToken, token);
    }

    public async Task<Result<AuthSuccessDto, FailureDto>> MeAsync(string refreshToken)
    {
        var storedRefreshToken = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (!IsRefreshTokenValid(storedRefreshToken)) return FailureDto.Unauthorized("Refresh token has expired.");

        var roles = await userManager.GetRolesAsync(storedRefreshToken!.User);
        var token = tokenProvider.CreateToken(storedRefreshToken.User, roles);

        return CreateAuthSuccessDto(storedRefreshToken.User, storedRefreshToken.Token, token);
    }

    private static bool IsRefreshTokenValid(RefreshTokenEntity? refreshTokenEntity)
    {
        return refreshTokenEntity is not null && refreshTokenEntity.ExpiryDate >= DateTime.UtcNow;
    }
}
