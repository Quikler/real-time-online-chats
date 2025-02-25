using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Providers;
using real_time_online_chats.Server.Services.Identity;

namespace WebAPI.UnitTests.Identity;

public class BaseIdentityServiceTests : BaseUnitTests
{
    protected virtual Mock<AppDbContext> DbContextMock { get; }
    protected virtual Mock<UserManager<UserEntity>> UserManagerMock { get; }

    protected virtual TokenProvider TokenProvider { get; }
    protected virtual JwtConfiguration JwtConfiguration { get; }

    protected virtual IdentityService IdentityService { get; }

    // static BaseIdentityServiceTests()
    // {
    //     JwtConfiguration = new JwtConfiguration
    //     {
    //         SecretKey = ",bGewnAe)0(7./{vwVnBnRK%S*xb08KP",
    //         ValidIssuer = "test",
    //         ValidAudience = "test",
    //         RefreshTokenLifetime = TimeSpan.FromDays(180),
    //         TokenLifetime = TimeSpan.FromSeconds(45),
    //     };

    //     TokenProvider = new TokenProvider(Options.Create(JwtConfiguration));
    // }

    public BaseIdentityServiceTests()
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

        IdentityService = new IdentityService(DbContextMock.Object, UserManagerMock.Object, TokenProvider, jwtConfigurationOptions);
    }
}