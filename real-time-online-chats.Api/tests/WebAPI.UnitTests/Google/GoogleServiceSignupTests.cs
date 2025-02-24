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
    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenEmailAlreadyRegistered()
    {
        // Arrange
        var payload = CreateGoogleJsonWebSignaturePayload();

        UserManagerMock.SetupFindByEmailAsync(CreateUserEntity());

        // Act
        var signupResult = await GoogleService.SignupAsync(payload);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Email is already registered.");

        UserManagerMock.VerifyFindByEmailAsync(payload.Email);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenCreateAsyncFails()
    {
        // Arrange
        var payload = CreateGoogleJsonWebSignaturePayload();

        UserManagerMock.SetupFindByEmailAsync(null);

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(IdentityResult.Failed([new IdentityError
            {
                Code = "SOME_ERROR_CODE",
                Description = "Some error happened.",
            }]));

        // Act
        var signupResult = await GoogleService.SignupAsync(payload);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Some error happened.");

        UserManagerMock.VerifyFindByEmailAsync(payload.Email);
        UserManagerMock.VerifyCreateAsync(payload.Email);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnEmailConfirmDto_WhenUserDoesntExist()
    {
        // Arrange
        var payload = CreateGoogleJsonWebSignaturePayload();

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
        var signupResult = await GoogleService.SignupAsync(payload);

        // Assert
        signupResult.IsSuccess.ShouldBeTrue();

        var matchResult = signupResult.Match(
            authSuccessDto => authSuccessDto,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.User.Id.ShouldBe(userIdToAssign);
        matchResult.Token.ShouldNotBeNullOrWhiteSpace();
        matchResult.RefreshToken.ShouldNotBeNullOrWhiteSpace();

        UserManagerMock.VerifyFindByEmailAsync(payload.Email);
        UserManagerMock.VerifyCreateAsync(payload.Email);
    }

    private static GoogleJsonWebSignature.Payload CreateGoogleJsonWebSignaturePayload(string email = TestEmail,
        string firstName = TestFirstName,
        string lastName = TestLastName,
        string picture = "https://picsum.photos/200/300")
    {
        return new GoogleJsonWebSignature.Payload
        {
            Email = email,
            Name = firstName,
            FamilyName = lastName,
            Picture = picture,
        };
    }
}