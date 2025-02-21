using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.Services.Identity;
using Shouldly;

namespace WebAPI.UnitTests.Identity;

public class IdentityServiceTests
{
    private const string Email = "test@test.com";
    private const string Password = "testtest";
    private const string Phone = "380777777777";

    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IUserStore<UserEntity>> _userStoreMock;

    private readonly IOptions<JwtConfiguration> _jwtConfiguration;

    public IdentityServiceTests()
    {
        _userStoreMock = new Mock<IUserStore<UserEntity>>();
        _userManagerMock = new Mock<UserManager<UserEntity>>(_userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _jwtConfiguration = Options.Create(new JwtConfiguration
        {
            SecretKey = "test",
            ValidIssuer = "test",
            ValidAudience = "test",
            RefreshTokenLifetime = TimeSpan.FromDays(180),
            TokenLifetime = TimeSpan.FromSeconds(45),
        });
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

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenEmailAlreadyRegistered()
    {
        // Arrange
        _userManagerMock
            .Setup(userManeger => userManeger.FindByEmailAsync(Email))
            .ReturnsAsync(new UserEntity
            {
                Email = Email,
            });

        var signupUserDto = CreateSignupUserDto(Email, Password, Phone);
        var identityService = new IdentityService(null!, _userManagerMock.Object, null!, _jwtConfiguration);

        // Act
        var signupResult = await identityService.SignupAsync(signupUserDto);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Email is already registered.");
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnError_WhenPasswordIsShort()
    {
        // Arrange
        _userManagerMock
            .Setup(userManeger => userManeger.FindByEmailAsync(Email))
            .ReturnsAsync((UserEntity?)null);

        _userManagerMock
            .Setup(userManager => userManager.CreateAsync(
                It.Is<UserEntity>(
                    userEntity => userEntity.Email == Email &&
                    userEntity.PhoneNumber == Phone &&
                    userEntity.UserName == Email
                ),
                It.Is<string>(password => password == "test"
            )))
            .ReturnsAsync(IdentityResult.Failed([new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Passwords must be at least 8 characters."
            }]));

        var signupUserDto = CreateSignupUserDto(Email, "test", Phone);
        var identityService = new IdentityService(null!, _userManagerMock.Object, null!, _jwtConfiguration);

        // Act
        var signupResult = await identityService.SignupAsync(signupUserDto);

        // Assert
        signupResult.IsSuccess.ShouldBeFalse();

        var matchResult = signupResult.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Passwords must be at least 8 characters.");
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnEmailConfirmDto_WhenUserDoesntExist()
    {
        // Arrange
        _userManagerMock
            .Setup(userManager => userManager.FindByEmailAsync(Email))
            .ReturnsAsync((UserEntity?)null);

        _userManagerMock
            .Setup(userManager => userManager.CreateAsync(
                It.Is<UserEntity>(
                    userEntity => userEntity.Email == Email &&
                    userEntity.PhoneNumber == Phone &&
                    userEntity.UserName == Email
                ),
                It.Is<string>(password => password == Password
            )))
            .Callback<UserEntity, string>((user, _) => user.Id = Guid.NewGuid())
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(userManager => userManager.GenerateEmailConfirmationTokenAsync(
                It.Is<UserEntity>(
                    userEntity => userEntity.Email == Email &&
                    userEntity.PhoneNumber == Phone &&
                    userEntity.UserName == Email
                )))
            .ReturnsAsync("emailToken");

        var signupUserDto = CreateSignupUserDto(Email, Password, Phone);
        var identityService = new IdentityService(null!, _userManagerMock.Object, null!, _jwtConfiguration);

        // Act
        var signupResult = await identityService.SignupAsync(signupUserDto);

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
    }
}