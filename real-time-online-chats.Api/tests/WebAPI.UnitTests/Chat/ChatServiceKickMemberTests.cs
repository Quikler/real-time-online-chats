using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;
using real_time_online_chats.Server.Common;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceKickMemberTests : BaseChatServiceTests
{
    private readonly UserEntity _owner;
    private readonly UserEntity _member;
    private readonly ChatEntity _ownedChat;

    public ChatServiceKickMemberTests()
    {
        _owner = Fixture.Create<UserEntity>();
        _ownedChat = Fixture.Build<ChatEntity>()
            .With(c => c.OwnerId, _owner.Id)
            .With(c => c.Owner, _owner)
            .Create();
        _member = Fixture.Create<UserEntity>();
    }

    [Fact]
    public async Task KickMemberAsync_ShouldReturnError_WhenUserDoesntOwnChat()
    {
        // Arrange
        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var kickResult = await ChatService.KickMemberAsync(_ownedChat.Id, _member.Id, _owner.Id);

        // Assert
        kickResult.IsSuccess.ShouldBeFalse();

        var matchResult = kickResult.Match(
            chatPreviewDto => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.Forbidden);
        matchResult.Errors.ShouldContain("User doesn't own this chat");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _owner.Id));
    }

    [Fact]
    public async Task KickMemberAsync_ShouldReturnError_WhenChatNotFound()
    {
        // Arrange
        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _owner.Id))
            .ReturnsAsync(true);

        // Act
        var kickResult = await ChatService.KickMemberAsync(_ownedChat.Id, _member.Id, _owner.Id);

        // Assert
        kickResult.IsSuccess.ShouldBeFalse();

        var matchResult = kickResult.Match(
            success => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.NotFound);
        matchResult.Errors.ShouldContain("Chat not found");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _owner.Id));
    }

    [Fact]
    public async Task KickMemberAsync_ShouldReturnError_WhenSaveChangesAsyncReturnsZero()
    {
        // Arrange
        List<ChatEntity> chatEntities = [_ownedChat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _owner.Id))
            .ReturnsAsync(true);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var kickResult = await ChatService.KickMemberAsync(_ownedChat.Id, _member.Id, _owner.Id);

        // Assert
        kickResult.IsSuccess.ShouldBeFalse();

        var matchResult = kickResult.Match(
            success => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.BadRequest);
        matchResult.Errors.ShouldContain("Cannot kick user");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _owner.Id));
        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task KickMemberAsync_ShouldReturnError_WhenEveryCheckPasses()
    {
        // Arrange
        List<ChatEntity> chatEntities = [_ownedChat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _owner.Id))
            .ReturnsAsync(true);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var kickResult = await ChatService.KickMemberAsync(_ownedChat.Id, _member.Id, _owner.Id);

        // Assert
        kickResult.IsSuccess.ShouldBeTrue();

        var matchResult = kickResult.Match(
            success => success,
            failure => throw new Exception("Should not be failure")
        );

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _owner.Id));
        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
}