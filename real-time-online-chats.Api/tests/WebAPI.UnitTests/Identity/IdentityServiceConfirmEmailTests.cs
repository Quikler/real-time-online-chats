using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Services.Identity;
using Shouldly;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceConfirmEmailTests : BaseIdentityServiceTests
{
    private readonly UserEntity _user;
    private readonly string _confirmToken;

    public IdentityServiceConfirmEmailTests()
    {
        _user = Fixture.Create<UserEntity>();
        _confirmToken = Guid.NewGuid().ToString();
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        UserManagerMock
            .Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var confirmEmailResult = await IdentityService.ConfirmEmailAsync(_user.Id, _confirmToken);

        // Assert
        confirmEmailResult.IsSuccess.ShouldBeFalse();
        var matchResult = confirmEmailResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("User not found.");

        UserManagerMock.Verify(userManager => userManager.FindByIdAsync(_user.Id.ToString()));
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnError_WhenConfirmEmailAsyncFailed()
    {
        // Arrange
        UserManagerMock
            .Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);

        UserManagerMock
            .Setup(userManager => userManager.ConfirmEmailAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed([new IdentityError
            {
                Code = "SOME_ERROR_CODE",
                Description = "Some error happened.",
            }]));

        // Act
        var confirmResult = await IdentityService.ConfirmEmailAsync(_user.Id, _confirmToken);

        // Assert
        confirmResult.IsSuccess.ShouldBeFalse();

        var matchResult = confirmResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Some error happened.");

        UserManagerMock.Verify(userManager => userManager.FindByIdAsync(_user.Id.ToString()));
        UserManagerMock.Verify(userManager => userManager.ConfirmEmailAsync(_user, _confirmToken));
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnTrue_WhenConfirmEmailAsyncSucceeded()
    {
        // Arrange
        UserManagerMock
            .Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);

        UserManagerMock
            .Setup(userManager => userManager.ConfirmEmailAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var confirmResult = await IdentityService.ConfirmEmailAsync(_user.Id, _confirmToken);

        // Assert
        confirmResult.IsSuccess.ShouldBeTrue();

        var matchResult = confirmResult.Match(
            success => success,
            failure => false
        );

        matchResult.ShouldBeTrue();

        UserManagerMock.Verify(userManager => userManager.FindByIdAsync(_user.Id.ToString()));
        UserManagerMock.Verify(userManager => userManager.ConfirmEmailAsync(_user, _confirmToken));
    }
}