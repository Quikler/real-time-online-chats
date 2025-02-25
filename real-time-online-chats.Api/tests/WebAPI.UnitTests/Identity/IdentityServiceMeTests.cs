using AutoFixture;
using MockQueryable.Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceMeTests : BaseIdentityServiceTests
{
    private readonly RefreshTokenEntity _refreshTokenEntity;

    public IdentityServiceMeTests()
    {
        _refreshTokenEntity = Fixture.Build<RefreshTokenEntity>()
            .With(r => r.Token, TokenProvider.GenerateRefreshToken())
            .Create();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MeAsync_ShouldReturnError_WhenTokenNotFoundOrExpired(bool isTokenFound)
    {
        // Arrange
        var refreshToken = TokenProvider.GenerateRefreshToken();
        List<RefreshTokenEntity> refreshTokens = [];
        if (isTokenFound)
        {
            _refreshTokenEntity.ExpiryDate = DateTime.UtcNow - TimeSpan.FromDays(1);
            _refreshTokenEntity.Token = refreshToken;
            refreshTokens.Add(_refreshTokenEntity);
        }

        var refreshTokensDbSetMock = refreshTokens.BuildMockDbSet();
        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokensDbSetMock.Object);

        // Act
        var meResult = await IdentityService.MeAsync(refreshToken);

        // Assert
        meResult.IsSuccess.ShouldBeFalse();

        var matchResult = meResult.Match(
            authSuccesDto => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Refresh token has expired.");
    }

    [Fact]
    public async Task MeAsync_ShouldReturnAuthSuccessDto_WhenEveryCheckPasses()
    {
        // Arrange
        var refreshToken = TokenProvider.GenerateRefreshToken();
        _refreshTokenEntity.ExpiryDate = DateTime.UtcNow.Add(JwtConfiguration.RefreshTokenLifetime);
        _refreshTokenEntity.Token = refreshToken;
        RefreshTokenEntity[] refreshTokens = [_refreshTokenEntity,];

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

        matchResult.User.Id.ShouldBe(_refreshTokenEntity.User.Id);
        matchResult.Token.ShouldNotBeNullOrWhiteSpace();
        matchResult.RefreshToken.ShouldBe(refreshToken);
    }
}