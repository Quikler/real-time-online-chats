using AutoFixture;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;
using WebAPI.UnitTests.Extensions;
using WebAPI.UnitTests.Google;

namespace WebApi.UnitTests.Google;

public class GoogleServiceSignupTests : BaseGoogleServiceTests
{
    private readonly UserEntity _user;
    private readonly GoogleJsonWebSignature.Payload _payload;

    public GoogleServiceSignupTests()
    {
        _user = Fixture.Create<UserEntity>();
        _payload = Fixture.Create<GoogleJsonWebSignature.Payload>();
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenEmailAlreadyRegistered()
    {
        // Arrange
        UserManagerMock.SetupFindByEmailAsync(_user);

        // Act
        var signupResult = await GoogleService.SignupAsync(_payload);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Email is already registered.");

        UserManagerMock.VerifyFindByEmailAsync(_payload.Email);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenCreateAsyncFails()
    {
        // Arrange
        UserManagerMock.SetupFindByEmailAsync(null);

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(IdentityResult.Failed([new IdentityError
            {
                Code = "SOME_ERROR_CODE",
                Description = "Some error happened.",
            }]));

        // Act
        var signupResult = await GoogleService.SignupAsync(_payload);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Some error happened.");

        UserManagerMock.VerifyFindByEmailAsync(_payload.Email);
        UserManagerMock.VerifyCreateAsync(_payload.Email);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnEmailConfirmDto_WhenUserDoesntExist()
    {
        // Arrange
        UserManagerMock.SetupFindByEmailAsync(null);

        var userIdToAssign = Guid.NewGuid();

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(
                It.IsAny<UserEntity>()))
            .Callback<UserEntity>((user) => user.Id = userIdToAssign)
            .ReturnsAsync(IdentityResult.Success);

        var refreshTokensDbSetMock = new Mock<DbSet<RefreshTokenEntity>>();
        DbContextMock
            .Setup(dbContext => dbContext.RefreshTokens)
            .Returns(refreshTokensDbSetMock.Object);

        // Act
        var signupResult = await GoogleService.SignupAsync(_payload);

        // Assert
        signupResult.IsSuccess.ShouldBeTrue();

        var matchResult = signupResult.Match(
            authSuccessDto => authSuccessDto,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.User.Id.ShouldBe(userIdToAssign);
        matchResult.Token.ShouldNotBeNullOrWhiteSpace();
        matchResult.RefreshToken.ShouldNotBeNullOrWhiteSpace();

        UserManagerMock.VerifyFindByEmailAsync(_payload.Email);
        UserManagerMock.VerifyCreateAsync(_payload.Email);
    }
}