using Microsoft.AspNetCore.Identity;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.Services.Identity;
using Shouldly;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceSignupTests : BaseIdentityServiceTests
{
    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenEmailAlreadyRegistered()
    {
        // Arrange
        UserManagerMock
            .Setup(userManeger => userManeger.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserEntity
            {
                Email = TestEmail,
            });

        var signupUserDto = CreateSignupUserDto(TestEmail, TestPassword, TestPhone);

        // Act
        var signupResult = await IdentityService.SignupAsync(signupUserDto);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Email is already registered.");

        UserManagerMock.Verify(userManager => userManager.FindByEmailAsync(signupUserDto.Email), Times.Once);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenCreateAsyncFails()
    {
        // Arrange
        UserManagerMock
            .Setup(userManeger => userManeger.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserEntity?)null);

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed([new IdentityError
            {
                Code = "SOME_ERROR_CODE",
                Description = "Some error happened.",
            }]));

        var signupUserDto = CreateSignupUserDto(TestEmail, "test", TestPhone);

        // Act
        var signupResult = await IdentityService.SignupAsync(signupUserDto);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Some error happened.");

        UserManagerMock.Verify(userManager => userManager.FindByEmailAsync(signupUserDto.Email), Times.Once);
        UserManagerMock.Verify(userManager => userManager.CreateAsync(
            It.Is<UserEntity>(userEntity =>
                userEntity.Email == signupUserDto.Email &&
                userEntity.PhoneNumber == signupUserDto.Phone
            ),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnEmailConfirmDto_WhenUserDoesntExist()
    {
        // Arrange
        UserManagerMock
            .Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserEntity?)null);

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(
                It.IsAny<UserEntity>(),
                It.IsAny<string>()))
            .Callback<UserEntity, string>((user, _) => user.Id = Guid.NewGuid())
            .ReturnsAsync(IdentityResult.Success);

        UserManagerMock
            .Setup(userManager => userManager.GenerateEmailConfirmationTokenAsync(
                It.IsAny<UserEntity>()))
            .ReturnsAsync("emailToken");

        var signupUserDto = CreateSignupUserDto(TestEmail, TestPassword, TestPhone);
        // Act
        var signupResult = await IdentityService.SignupAsync(signupUserDto);

        // Assert
        signupResult.IsSuccess.ShouldBeTrue();

        var matchResult = signupResult.Match(
            success =>
            {
                success.UserId.ShouldNotBe(default);
                success.Token.ShouldBe("emailToken");
                return true;
            },
            failure => false
        );

        matchResult.ShouldBeTrue();

        UserManagerMock.Verify(userManager => userManager.FindByEmailAsync(signupUserDto.Email), Times.Once);
        UserManagerMock.Verify(userManager => userManager.CreateAsync(
            It.Is<UserEntity>(userEntity =>
                userEntity.Email == signupUserDto.Email &&
                userEntity.PhoneNumber == signupUserDto.Phone
            ),
            signupUserDto.Password
        ), Times.Once);
        UserManagerMock.Verify(userManager => userManager.GenerateEmailConfirmationTokenAsync(
            It.Is<UserEntity>(userEntity =>
                userEntity.Email == signupUserDto.Email &&
                userEntity.PhoneNumber == signupUserDto.Phone
            )
        ), Times.Once);
    }

    private static SignupUserDto CreateSignupUserDto(string email, string password, string phone)
    {
        return new SignupUserDto
        {
            Email = email,
            Password = password,
            Phone = phone,
        };
    }
}