using Microsoft.EntityFrameworkCore;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using Shouldly;
using WebAPI.UnitTests.Extensions;

namespace WebAPI.UnitTests.Identity
{
    public class IdentityServiceLoginTests : BaseIdentityServiceTests
    {
        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);

            UserManagerMock.SetupFindByEmailAsync(null);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Invalid email or password.");

            UserManagerMock.VerifyFindByEmailAsync(loginUserDto.Email);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);
            var user = CreateUserEntity();

            UserManagerMock.SetupFindByEmailAsync(user);
            UserManagerMock.SetupCheckPasswordAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Invalid email or password.");

            UserManagerMock.VerifyFindByEmailAsync(loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(user, loginUserDto.Password);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenEmailNotConfirmed()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);
            var user = CreateUserEntity();

            UserManagerMock.SetupFindByEmailAsync(user);
            UserManagerMock.SetupCheckPasswordAsync(true);
            UserManagerMock.SetupIsEmailConfirmedAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Email is not confirmed.");

            UserManagerMock.VerifyFindByEmailAsync(loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(user, loginUserDto.Password);
            UserManagerMock.VerifyIsEmailConfirmedAsync(user);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenAccountIsLocked()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);
            var user = CreateUserEntity();

            UserManagerMock.SetupFindByEmailAsync(user);
            UserManagerMock.SetupCheckPasswordAsync(true);
            UserManagerMock.SetupIsEmailConfirmedAsync(true);
            UserManagerMock.SetupIsLockedOutAsync(true);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Account is locked. Please try again later.");

            UserManagerMock.VerifyFindByEmailAsync(loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(user, loginUserDto.Password);
            UserManagerMock.VerifyIsEmailConfirmedAsync(user);
            UserManagerMock.VerifyIsLockedOutAsync(user);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnGenerateSuccessDtoForUser_WhenEveryCheckPasses()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);
            var user = CreateUserEntity();

            var refreshTokensDbSetMock = new Mock<DbSet<RefreshTokenEntity>>();
            DbContextMock
                .Setup(dbContext => dbContext.RefreshTokens)
                .Returns(refreshTokensDbSetMock.Object);

            UserManagerMock.SetupFindByEmailAsync(user);
            UserManagerMock.SetupCheckPasswordAsync(true);
            UserManagerMock.SetupIsEmailConfirmedAsync(true);
            UserManagerMock.SetupIsLockedOutAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeTrue();

            var matchResult = loginResult.Match(
                authSuccessDto => authSuccessDto,
                failure => throw new Exception("Should not be failure.")
            );

            matchResult.User.Id.ShouldBe(user.Id);
            matchResult.Token.ShouldNotBeNullOrWhiteSpace();
            matchResult.RefreshToken.ShouldNotBeNullOrWhiteSpace();

            UserManagerMock.VerifyFindByEmailAsync(loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(user, loginUserDto.Password);
            UserManagerMock.VerifyIsEmailConfirmedAsync(user);
            UserManagerMock.VerifyIsLockedOutAsync(user);
        }

        private static LoginUserDto CreateLoginUserDto(string email, string password) => new()
        {
            Email = email,
            Password = password,
        };
    }
}