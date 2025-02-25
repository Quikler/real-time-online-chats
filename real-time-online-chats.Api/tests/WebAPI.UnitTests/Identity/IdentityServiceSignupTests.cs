using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.Services.Identity;
using Shouldly;
using WebAPI.UnitTests.Extensions;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceSignupTests : BaseIdentityServiceTests
{
    private readonly SignupUserDto _signupUserDto;
    private readonly UserEntity _user;

    public IdentityServiceSignupTests()
    {
        _signupUserDto = Fixture.Create<SignupUserDto>();
        _user = Fixture.Build<UserEntity>()
            .With(u => u.Email, _signupUserDto.Email)
            .With(u => u.FirstName, _signupUserDto.FirstName)
            .With(u => u.LastName, _signupUserDto.LastName)
            .With(u => u.PhoneNumber, _signupUserDto.Phone)
            .Create();
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenEmailAlreadyRegistered()
    {
        // Arrange
        UserManagerMock.SetupFindByEmailAsync(_user);

        // Act
        var signupResult = await IdentityService.SignupAsync(_signupUserDto);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Email is already registered.");

        UserManagerMock.VerifyFindByEmailAsync(_signupUserDto.Email);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenCreateAsyncFails()
    {
        // Arrange
        UserManagerMock.SetupFindByEmailAsync(null);

        UserManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed([new IdentityError
            {
                Code = "SOME_ERROR_CODE",
                Description = "Some error happened.",
            }]));

        // Act
        var signupResult = await IdentityService.SignupAsync(_signupUserDto);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Some error happened.");

        UserManagerMock.VerifyFindByEmailAsync(_signupUserDto.Email);
        UserManagerMock.VerifyCreateAsync(_signupUserDto.Email, _signupUserDto.Phone, _signupUserDto.Password);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnEmailConfirmDto_WhenUserDoesntExist()
    {
        // Arrange
        UserManagerMock.SetupFindByEmailAsync(null);

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

        // Act
        var signupResult = await IdentityService.SignupAsync(_signupUserDto);

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

        UserManagerMock.VerifyFindByEmailAsync(_signupUserDto.Email);
        UserManagerMock.VerifyCreateAsync(_signupUserDto.Email, _signupUserDto.Phone, _signupUserDto.Password);
        UserManagerMock.Verify(userManager => userManager.GenerateEmailConfirmationTokenAsync(
            It.Is<UserEntity>(userEntity => userEntity.Email == _signupUserDto.Email)
        ));
    }
}