using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Providers;

namespace real_time_online_chats.Server.Services.Identity;

public class IdentityService(UserManager<UserEntity> userManager, IOptions<JwtConfiguration> jwtConfiguration,
    AppDbContext dbContext, TokenProvider tokenProvider) : IIdentityService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly TokenProvider _tokenProvider = tokenProvider;

    public async Task<AuthResult> SignupAsync(SignupUser signupUser)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(signupUser.Email);
            if (existingUser is not null)
            {
                return new AuthResult
                {
                    Errors = ["Email is already registered."]
                };
            }

            var newUser = new UserEntity
            {
                Email = signupUser.Email,
                UserName = signupUser.Email,
            };

            var createdUserResult = await _userManager.CreateAsync(newUser, signupUser.Password);

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

    public async Task<AuthResult> LoginAsync(LoginUser loginUser)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginUser.Password))
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

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var storedRefreshToken = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow) return new AuthResult
            {
                Errors = ["Refresh token has expired", $"refreskToken: {refreshToken} - Null: {storedRefreshToken is null}"],
            };

            var newRefreshToken = _tokenProvider.GenerateRefreshToken();
            storedRefreshToken.Token = newRefreshToken;
            storedRefreshToken.ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime);

            await _dbContext.SaveChangesAsync();

            return new AuthResult
            {
                Succeded = true,
                RefreshToken = newRefreshToken,
                Token = _tokenProvider.CreateJwtSecurityToken(storedRefreshToken.User),
                User = new UserResult
                {
                    Id = storedRefreshToken.UserId,
                    Email = storedRefreshToken.User.Email!,
                    FirstName = storedRefreshToken.User.FirstName,
                    LastName = storedRefreshToken.User.LastName,
                },
            };
        }
        catch
        {
            return new AuthResult
            {
                Errors = ["An unexpected error occurred. Please try again later."]
            };
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

    public async Task<AuthResult> MeAsync(string refreshToken)
    {
        var rToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (rToken is null) return new AuthResult
        {
            Errors = ["Refresh token is not found. Token has expired"],
        };

        return new AuthResult
        {
            Succeded = true,
            Token = _tokenProvider.CreateJwtSecurityToken(rToken.User),
            RefreshToken = rToken.Token,
            User = new UserResult
            {
                Id = rToken.UserId,
                FirstName = rToken.User.FirstName,
                LastName = rToken.User.LastName,
                Email = rToken.User.Email!,
            }
        };
    }
}