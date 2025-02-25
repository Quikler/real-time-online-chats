using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Providers;
using real_time_online_chats.Server.Services.Google;

namespace WebAPI.UnitTests.Google;

public class BaseGoogleServiceTests : BaseUnitTests
{
    protected virtual Mock<AppDbContext> DbContextMock { get; }
    protected virtual Mock<UserManager<UserEntity>> UserManagerMock { get; }

    protected virtual TokenProvider TokenProvider { get; }
    protected virtual GoogleConfiguration GoogleConfiguration { get; }
    protected virtual JwtConfiguration JwtConfiguration { get; }

    protected virtual GoogleService GoogleService { get; }

    public BaseGoogleServiceTests()
    {
        DbContextMock = new Mock<AppDbContext>();

        var userStoreMock = new Mock<IUserStore<UserEntity>>();
        UserManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        JwtConfiguration = new JwtConfiguration
        {
            SecretKey = ",bGewnAe)0(7./{vwVnBnRK%S*xb08KP",
            ValidIssuer = "test",
            ValidAudience = "test",
            RefreshTokenLifetime = TimeSpan.FromDays(180),
            TokenLifetime = TimeSpan.FromSeconds(45),
        };

        var jwtConfigurationOptions = Options.Create(JwtConfiguration);

        TokenProvider = new TokenProvider(jwtConfigurationOptions);

        GoogleConfiguration = new GoogleConfiguration
        {
            ClientId = "9251370213329-7714pt8hnh85qe6g5446h32tf3q2v1a.apps.googleusercontent.com",
            ClientSecret = "0a5a2ddc-c5e0-4498-bf15-2c71ce4989e9",
        };

        var googleConfigurationOptions = Options.Create(GoogleConfiguration);

        GoogleService = new GoogleService(googleConfigurationOptions,
            DbContextMock.Object,
            UserManagerMock.Object,
            TokenProvider, jwtConfigurationOptions);
    }
}