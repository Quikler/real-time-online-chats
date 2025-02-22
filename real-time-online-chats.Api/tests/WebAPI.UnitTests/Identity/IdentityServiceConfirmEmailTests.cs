using Microsoft.AspNetCore.Identity;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Services.Identity;
using Shouldly;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceConfirmEmailTests : BaseIdentityServiceTests
{
    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "emailToken";

        UserManagerMock
            .Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var confirmEmailResult = await IdentityService.ConfirmEmailAsync(userId, token);

        // Assert
        confirmEmailResult.IsSuccess.ShouldBeFalse();
        var matchResult = confirmEmailResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("User not found.");

        UserManagerMock.Verify(userManager => userManager.FindByIdAsync(userId.ToString()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnError_WhenConfirmEmailAsyncFailed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "emailToken";

        UserManagerMock
            .Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserEntity
            {
                Id = userId,
            });

        UserManagerMock
            .Setup(userManager => userManager.ConfirmEmailAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed([new IdentityError
            {
                Code = "SOME_ERROR_CODE",
                Description = "Some error happened.",
            }]));

        // Act
        var confirmResult = await IdentityService.ConfirmEmailAsync(userId, token);

        // Assert
        confirmResult.IsSuccess.ShouldBeFalse();

        var matchResult = confirmResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Some error happened.");

        UserManagerMock.Verify(userManager => userManager.FindByIdAsync(userId.ToString()), Times.Once);
        UserManagerMock.Verify(userManager => userManager.ConfirmEmailAsync(
            It.Is<UserEntity>(userEntity => userEntity.Id == userId), token), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnTrue_WhenConfirmEmailAsyncSucceeded()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "emailToken";

        UserManagerMock
            .Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserEntity
            {
                Id = userId,
            });

        UserManagerMock
            .Setup(userManager => userManager.ConfirmEmailAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var confirmResult = await IdentityService.ConfirmEmailAsync(userId, token);

        // Assert
        confirmResult.IsSuccess.ShouldBeTrue();

        var matchResult = confirmResult.Match(
            success => success,
            failure => false
        );

        matchResult.ShouldBeTrue();

        UserManagerMock.Verify(userManager => userManager.FindByIdAsync(userId.ToString()), Times.Once);
        UserManagerMock.Verify(userManager => userManager.ConfirmEmailAsync(
            It.Is<UserEntity>(userEntity => userEntity.Id == userId), token), Times.Once);
    }
}