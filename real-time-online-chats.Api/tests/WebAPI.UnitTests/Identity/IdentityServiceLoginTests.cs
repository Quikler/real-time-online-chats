using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using Shouldly;

namespace WebAPI.UnitTests.Identity
{
    public class IdentityServiceLoginTests : BaseIdentityServiceTests
    {
        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenUserNotFound()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);

            UserManagerMock
                .Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Invalid email or password.");

            UserManagerMock.Verify(userManager => userManager.FindByEmailAsync(loginUserDto.Email), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
            };

            UserManagerMock
                .Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            UserManagerMock
                .Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Invalid email or password.");

            UserManagerMock.Verify(userManager => userManager.FindByEmailAsync(loginUserDto.Email), Times.Once);
            UserManagerMock.Verify(userManager => userManager.CheckPasswordAsync(user, loginUserDto.Password), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenEmailNotConfirmed()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
            };

            UserManagerMock
                .Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            UserManagerMock
                .Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            UserManagerMock
                .Setup(userManager => userManager.IsEmailConfirmedAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync(false);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Email is not confirmed.");

            UserManagerMock.Verify(userManager => userManager.FindByEmailAsync(loginUserDto.Email), Times.Once);
            UserManagerMock.Verify(userManager => userManager.CheckPasswordAsync(user, loginUserDto.Password), Times.Once);
            UserManagerMock.Verify(userManager => userManager.IsEmailConfirmedAsync(user), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenAccountIsLocked()
        {
            // Arrange
            var loginUserDto = CreateLoginUserDto(TestEmail, TestPassword);
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
            };

            UserManagerMock
                .Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            UserManagerMock
                .Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            UserManagerMock
                .Setup(userManager => userManager.IsEmailConfirmedAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync(true);

            UserManagerMock
                .Setup(userManager => userManager.IsLockedOutAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync(true);

            // Act
            var loginResult = await IdentityService.LoginAsync(loginUserDto);

            // Assert
            loginResult.IsSuccess.ShouldBeFalse();

            var matchResult = loginResult.Match(
                success => [],
                failure => failure.Errors
            );

            matchResult.ShouldContain("Account is locked. Please try again later.");

            UserManagerMock.Verify(userManager => userManager.FindByEmailAsync(loginUserDto.Email), Times.Once);
            UserManagerMock.Verify(userManager => userManager.CheckPasswordAsync(user, loginUserDto.Password), Times.Once);
            UserManagerMock.Verify(userManager => userManager.IsEmailConfirmedAsync(user), Times.Once);
            UserManagerMock.Verify(userManager => userManager.IsLockedOutAsync(user), Times.Once);
        }

        private static LoginUserDto CreateLoginUserDto(string email, string password)
        {
            return new LoginUserDto
            {
                Email = email,
                Password = password,
            };
        }
    }
}