using MockQueryable.Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceMeTests : BaseIdentityServiceTests
{
    [Fact]
    public async Task MeAsync_ShouldReturnError_WhenTokenNotFound()
    {
        // Arrange
        var refreshToken = TokenProvider.GenerateRefreshToken();
        List<RefreshTokenEntity> refreshTokens = [];

        var refreshTokensDbSetMock = refreshTokens.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokensDbSetMock.Object);

        // Act
        var meResult = await IdentityService.MeAsync(refreshToken);

        // Assert
        meResult.IsSuccess.ShouldBeFalse();

        var matchResult = meResult.Match(
            authSuccessDto => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Refresh token has expired.");
    }

    [Fact]
    public async Task MeAsync_ShouldReturnError_WhenTokenExpired()
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
        var meResult = await IdentityService.MeAsync(refreshToken);

        // Assert
        meResult.IsSuccess.ShouldBeFalse();

        var matchResult = meResult.Match(
            authSuccessDto => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Refresh token has expired.");
    }

    [Fact]
    public async Task MeAsync_ShouldReturnAuthSuccessDto_WhenEveryCheckPasses()
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
        var refreshResult = await IdentityService.MeAsync(refreshToken);

        // Assert
        refreshResult.IsSuccess.ShouldBeTrue();

        var matchResult = refreshResult.Match(
            authSuccessDto => authSuccessDto,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.User.Id.ShouldBe(user.Id);
        matchResult.Token.ShouldNotBeNullOrWhiteSpace();
        matchResult.RefreshToken.ShouldBe(refreshToken);
    }
}