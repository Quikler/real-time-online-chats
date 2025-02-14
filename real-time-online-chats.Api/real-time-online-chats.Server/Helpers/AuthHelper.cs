using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Helpers;

public static class AuthHelper
{
    public static async Task<AuthSuccessDto> GenerateAuthSuccessDtoForUserAsync(
        UserEntity user,
        TokenProvider tokenProvider,
        AppDbContext dbContext,
        TimeSpan refreshTokenLifetime, IEnumerable<string> roles)
    {
        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.Add(refreshTokenLifetime),
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();

        return CreateAuthSuccess(user, refreshToken.Token, tokenProvider.CreateToken(user, roles));
    }

    public static AuthSuccessDto CreateAuthSuccess(UserEntity user, string refreshToken, string token)
    {
        return new AuthSuccessDto
        {
            Token = token,
            RefreshToken = refreshToken,
            User = new UserGlobalDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
            }
        };
    }
}