using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceRefreshTokenTests : BaseIdentityServiceTests
{
    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnError_WhenTokenNotFound()
    {
        // Arrange
        var refreshToken = TokenProvider.GenerateRefreshToken();
        List<RefreshTokenEntity> refreshTokens = [];

        var refreshTokensDbSetMock = refreshTokens.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokensDbSetMock.Object);

        // Act
        var refreshResult = await IdentityService.RefreshTokenAsync(refreshToken);

        // Assert
        refreshResult.IsSuccess.ShouldBeFalse();

        var matchResult = refreshResult.Match(
            authSuccessDto => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Refresh token has expired.");
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnError_WhenTokenExpired()
    {
        // Arrange
        var user = CreateUserEntity();
        var refreshToken = TokenProvider.GenerateRefreshToken();
        List<RefreshTokenEntity> refreshTokens = [
            new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                ExpiryDate = DateTime.UtcNow - TimeSpan.FromDays(1),
                Token = refreshToken,
                User = user,
                UserId = user.Id,
            }
        ];

        var refreshTokensDbSetMock = refreshTokens.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokensDbSetMock.Object);

        // Act
        var refreshResult = await IdentityService.RefreshTokenAsync(refreshToken);

        // Assert
        refreshResult.IsSuccess.ShouldBeFalse();

        var matchResult = refreshResult.Match(
            authSuccessDto => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Refresh token has expired.");
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnAuthSuccessDto_WhenEveryCheckPasses()
    {
        // Arrange
        var user = CreateUserEntity();
        var refreshToken = TokenProvider.GenerateRefreshToken();
        List<RefreshTokenEntity> refreshTokens = [
            new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                ExpiryDate = DateTime.UtcNow.Add(JwtConfiguration.RefreshTokenLifetime),
                Token = refreshToken,
                User = user,
                UserId = user.Id,
            }
        ];

        var refreshTokensDbSetMock = refreshTokens.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokensDbSetMock.Object);

        // Act
        var refreshResult = await IdentityService.RefreshTokenAsync(refreshToken);

        // Assert
        refreshResult.IsSuccess.ShouldBeTrue();

        var matchResult = refreshResult.Match(
            authSuccessDto => authSuccessDto,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.User.Id.ShouldBe(user.Id);
        matchResult.Token.ShouldNotBeNullOrWhiteSpace();
        matchResult.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }
}