using AutoFixture;
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
        private readonly LoginUserDto _loginUserDto;
        private readonly UserEntity _user;

        public IdentityServiceLoginTests()
        {
            _loginUserDto = Fixture.Create<LoginUserDto>();
            _user = Fixture.Build<UserEntity>()
                .With(u => u.Email, _loginUserDto.Email)
                .Create();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            UserManagerMock.SetupFindByEmailAsync(null);

            // Act
            var loginResult = await IdentityService.LoginAsync(_loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Invalid email or password.");

            UserManagerMock.VerifyFindByEmailAsync(_loginUserDto.Email);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenPasswordIsInvalid()
        {
            // Arrange
            UserManagerMock.SetupFindByEmailAsync(_user);
            UserManagerMock.SetupCheckPasswordAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(_loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Invalid email or password.");

            UserManagerMock.VerifyFindByEmailAsync(_loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(_user, _loginUserDto.Password);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenEmailNotConfirmed()
        {
            // Arrange
            UserManagerMock.SetupFindByEmailAsync(_user);
            UserManagerMock.SetupCheckPasswordAsync(true);
            UserManagerMock.SetupIsEmailConfirmedAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(_loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Email is not confirmed.");

            UserManagerMock.VerifyFindByEmailAsync(_loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(_user, _loginUserDto.Password);
            UserManagerMock.VerifyIsEmailConfirmedAsync(_user);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenAccountIsLocked()
        {
            // Arrange
            UserManagerMock.SetupFindByEmailAsync(_user);
            UserManagerMock.SetupCheckPasswordAsync(true);
            UserManagerMock.SetupIsEmailConfirmedAsync(true);
            UserManagerMock.SetupIsLockedOutAsync(true);

            // Act
            var loginResult = await IdentityService.LoginAsync(_loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Account is locked. Please try again later.");

            UserManagerMock.VerifyFindByEmailAsync(_loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(_user, _loginUserDto.Password);
            UserManagerMock.VerifyIsEmailConfirmedAsync(_user);
            UserManagerMock.VerifyIsLockedOutAsync(_user);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnGenerateSuccessDtoForUser_WhenEveryCheckPasses()
        {
            // Arrange
            var refreshTokensDbSetMock = new Mock<DbSet<RefreshTokenEntity>>();
            DbContextMock
                .Setup(dbContext => dbContext.RefreshTokens)
                .Returns(refreshTokensDbSetMock.Object);

            UserManagerMock.SetupFindByEmailAsync(_user);
            UserManagerMock.SetupCheckPasswordAsync(true);
            UserManagerMock.SetupIsEmailConfirmedAsync(true);
            UserManagerMock.SetupIsLockedOutAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(_loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeTrue();

            var matchResult = loginResult.Match(
                authSuccessDto => authSuccessDto,
                failure => throw new Exception("Should not be failure.")
            );

            matchResult.User.Id.ShouldBe(_user.Id);
            matchResult.Token.ShouldNotBeNullOrWhiteSpace();
            matchResult.RefreshToken.ShouldNotBeNullOrWhiteSpace();

            UserManagerMock.VerifyFindByEmailAsync(_loginUserDto.Email);
            UserManagerMock.VerifyCheckPasswordAsync(_user, _loginUserDto.Password);
            UserManagerMock.VerifyIsEmailConfirmedAsync(_user);
            UserManagerMock.VerifyIsLockedOutAsync(_user);
        }
    }
}