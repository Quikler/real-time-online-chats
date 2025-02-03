using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Helpers;

public static class AuthHelper
{
    public static async Task<AuthSuccess> GenerateAuthResultForUserAsync(
        UserEntity user,
        TokenProvider tokenProvider,
        AppDbContext dbContext,
        TimeSpan refreshTokenLifetime)
    {
        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.Add(refreshTokenLifetime),
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();

        return CreateAuthSuccess(user, refreshToken.Token, tokenProvider);
    }

    public static AuthSuccess CreateAuthSuccess(UserEntity user, string refreshToken, TokenProvider tokenProvider)
    {
        return new AuthSuccess
        {
            Token = tokenProvider.CreateToken(user),
            RefreshToken = refreshToken,
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