using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceUserJoinChatTests : BaseChatServiceTests
{
    private readonly UserEntity _user;
    private readonly ChatEntity _chat;

    public ChatServiceUserJoinChatTests()
    {
        _user = Fixture.Create<UserEntity>();
        _chat = Fixture.Create<ChatEntity>();
    }

    [Fact]
    public async Task UserJoinChatAsync_ShouldReturnUserWithTrueFlag_WhenUserAlreadyInChat()
    {
        // Arrange
        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id))
            .ReturnsAsync(true);

        List<UserEntity> userEntities = [_user];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        // Act
        var joinResult = await ChatService.UserJoinChatAsync(_chat.Id, _user.Id);

        // Assert
        joinResult.IsSuccess.ShouldBeTrue();

        var (user, isAlreadyInChat) = joinResult.Match(
            userTuple => userTuple,
            failure => throw new Exception("Should not be failure")
        );

        user.Id.ShouldBe(_user.Id);
        user.Email.ShouldBe(_user.Email);

        isAlreadyInChat.ShouldBeTrue();

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id));
    }

    [Fact]
    public async Task UserJoinChatAsync_ShouldReturnError_WhenChatNotFound()
    {
        // Arrange
        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id))
            .ReturnsAsync(false);

        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var joinResult = await ChatService.UserJoinChatAsync(_chat.Id, _user.Id);

        // Assert
        joinResult.IsSuccess.ShouldBeFalse();

        var matchResult = joinResult.Match(
            userTuple => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.NotFound);
        matchResult.Errors.ShouldContain("Chat not found");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id));
    }

    [Fact]
    public async Task UserJoinChatAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id))
            .ReturnsAsync(false);

        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        List<UserEntity> userEntities = [];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        // Act
        var joinResult = await ChatService.UserJoinChatAsync(_chat.Id, _user.Id);

        // Assert
        joinResult.IsSuccess.ShouldBeFalse();

        var matchResult = joinResult.Match(
            userTuple => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.NotFound);
        matchResult.Errors.ShouldContain("User not found");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id));
        userEntitiesDbSetMock.Verify(usersDbSet => usersDbSet.FindAsync(_user.Id));
    }

    [Fact]
    public async Task UserJoinChatAsync_ShouldReturnError_WhenSaveChangesAsyncReturnsZero()
    {
        // Arrange
        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id))
            .ReturnsAsync(false);

        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        List<UserEntity> userEntities = [_user];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        userEntitiesDbSetMock
            .Setup(usersDbSet => usersDbSet.FindAsync(It.IsAny<object?[]?>()))
            .ReturnsAsync(_user);

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var joinResult = await ChatService.UserJoinChatAsync(_chat.Id, _user.Id);

        // Assert
        joinResult.IsSuccess.ShouldBeFalse();

        var matchResult = joinResult.Match(
            userTuple => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.BadRequest);
        matchResult.Errors.ShouldContain("Cannot join chat");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id));
        userEntitiesDbSetMock.Verify(usersDbSet => usersDbSet.FindAsync(_user.Id));
        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UserJoinChatAsync_ShouldReturnUser_WhenEveryCheckPass()
    {
        // Arrange
        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id))
            .ReturnsAsync(false);

        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        List<UserEntity> userEntities = [_user];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        userEntitiesDbSetMock
            .Setup(usersDbSet => usersDbSet.FindAsync(It.IsAny<object?[]?>()))
            .ReturnsAsync(_user);

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var joinResult = await ChatService.UserJoinChatAsync(_chat.Id, _user.Id);

        // Assert
        joinResult.IsSuccess.ShouldBeTrue();

        var (user, isAlreadyInChat) = joinResult.Match(
            userTuple => userTuple,
            failure => throw new Exception("Should not be failure")
        );

        isAlreadyInChat.ShouldBeFalse();
        user.Id.ShouldBe(_user.Id);
        user.Email.ShouldBe(_user.Email);

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserExistInChatAsync(_chat.Id, _user.Id));
        userEntitiesDbSetMock.Verify(usersDbSet => usersDbSet.FindAsync(_user.Id));
        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
}