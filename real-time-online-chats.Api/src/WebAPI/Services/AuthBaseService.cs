using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Services;

public class AuthBaseService(UserManager<UserEntity> userManager, 
    TokenProvider tokenProvider, 
    IOptions<JwtConfiguration> jwtConfiguration,
    AppDbContext dbContext)
{
    protected readonly UserManager<UserEntity> userManager = userManager;
    protected readonly TokenProvider tokenProvider = tokenProvider;
    protected readonly JwtConfiguration jwtConfiguration = jwtConfiguration.Value;
    protected readonly AppDbContext dbContext = dbContext;

    protected async Task<AuthSuccessDto> GenerateAuthSuccessDtoForUserAsync(UserEntity user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.Add(jwtConfiguration.RefreshTokenLifetime),
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();

        return CreateAuthSuccessDto(user, refreshToken.Token, tokenProvider.CreateToken(user, roles));
    }

    protected static AuthSuccessDto CreateAuthSuccessDto(UserEntity user, string refreshToken, string token) => new()
    {
        RefreshToken = refreshToken,
        Token = token,
        User = user.ToUserGlobal(),
    };
}