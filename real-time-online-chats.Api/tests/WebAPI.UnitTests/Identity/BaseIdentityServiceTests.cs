using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Providers;
using real_time_online_chats.Server.Services.Identity;

namespace WebAPI.UnitTests.Identity;

public class BaseIdentityServiceTests
{
    protected const string TestEmail = "test@test.com";
    protected const string TestPassword = "testtest";
    protected const string TestPhone = "380777777777";

    protected virtual Mock<AppDbContext> DbContextMock { get; }
    protected virtual Mock<UserManager<UserEntity>> UserManagerMock { get; }

    protected virtual TokenProvider TokenProvider { get; }
    protected virtual IOptions<JwtConfiguration> JwtConfiguration { get; }

    protected virtual IdentityService IdentityService { get; }

    public BaseIdentityServiceTests()
    {
        DbContextMock = new Mock<AppDbContext>();

        var userStoreMock = new Mock<IUserStore<UserEntity>>();
        UserManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        JwtConfiguration = Options.Create(new JwtConfiguration
        {
            SecretKey = ",bGewnAe)0(7./{vwVnBnRK%S*xb08KP",
            ValidIssuer = "test",
            ValidAudience = "test",
            RefreshTokenLifetime = TimeSpan.FromDays(180),
            TokenLifetime = TimeSpan.FromSeconds(45),
        });

        TokenProvider = new TokenProvider(JwtConfiguration);

        IdentityService = new IdentityService(DbContextMock.Object, UserManagerMock.Object, TokenProvider, JwtConfiguration);
    }

    protected static UserEntity CreateUserEntity() => new()
    {
        Id = Guid.NewGuid(),
        Email = TestEmail,
        UserName = TestEmail,
        PhoneNumber = TestPhone,
    };
}