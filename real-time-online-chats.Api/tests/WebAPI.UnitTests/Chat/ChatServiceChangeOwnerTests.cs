using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;
using real_time_online_chats.Server.Common;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceChangeOwnerTests : BaseChatServiceTests
{
    private readonly UserEntity _owner;
    private readonly ChatEntity _chat;
    private readonly UserEntity _member;

    public ChatServiceChangeOwnerTests()
    {
        _owner = Fixture.Build<UserEntity>()
            .Without(o => o.OwnedChats)
            .Create();

        _member = Fixture.Build<UserEntity>()
            .Without(m => m.OwnedChats)
            .Create();

        _chat = Fixture.Build<ChatEntity>()
            .With(c => c.Members, [_member])
            .With(c => c.Owner, _owner)
            .With(c => c.OwnerId, _owner.Id)
            .Create();

        _owner.OwnedChats = [_chat];

        _member.MemberChats = [_chat];
    }

    [Fact]
    public async Task ChangeOwnerAsync_ShouldReturnError_WhenUserDoesntOwnChat()
    {
        // Arrange
        var randomChatId = Guid.NewGuid();

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var changeOwnerResult = await ChatService.ChangeOwnerAsync(randomChatId, _member.Id, _owner.Id);

        // Assert
        changeOwnerResult.IsSuccess.ShouldBeFalse();

        var matchResult = changeOwnerResult.Match(
            chatPreviewDto => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.Forbidden);
        matchResult.Errors.ShouldContain("User doesn't own this chat");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(randomChatId, _owner.Id));
    }

    [Fact]
    public async Task ChangeOwnerAsync_ShouldReturnError_WhenChatNotFound()
    {
        // Arrange
        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id))
            .ReturnsAsync(true);

        // Act
        var changeOwnerResult = await ChatService.ChangeOwnerAsync(_chat.Id, _member.Id, _owner.Id);

        // Assert
        changeOwnerResult.IsSuccess.ShouldBeFalse();

        var matchResult = changeOwnerResult.Match(
            success => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.NotFound);
        matchResult.Errors.ShouldContain("Chat not found");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id));
    }

    [Fact]
    public async Task ChangeOwnerAsync_ShouldReturnError_WhenNoMembersFound()
    {
        // Arrange
        _chat.Members = [];

        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id))
            .ReturnsAsync(true);

        // Act
        var changeOwnerResult = await ChatService.ChangeOwnerAsync(_chat.Id, _member.Id, _owner.Id);

        // Assert
        changeOwnerResult.IsSuccess.ShouldBeFalse();

        var matchResult = changeOwnerResult.Match(
            success => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.NotFound);
        matchResult.Errors.ShouldContain("User not found");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id));
    }

    [Fact]
    public async Task ChangeOwnerAsync_ShouldReturnError_WhenSaveChangesAsyncReturnsZero()
    {
        // Arrange
        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id))
            .ReturnsAsync(true);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var changeOwnerResult = await ChatService.ChangeOwnerAsync(_chat.Id, _member.Id, _owner.Id);

        // Assert
        changeOwnerResult.IsSuccess.ShouldBeFalse();

        var matchResult = changeOwnerResult.Match(
            success => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.BadRequest);
        matchResult.Errors.ShouldContain("Cannot change owner");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id));
        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ChangeOwnerAsync_ShouldReturnTrue_WhenEveryCheckPasses()
    {
        // Arrange
        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id))
            .ReturnsAsync(true);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _chat.Members.Remove(_member);
                _chat.Members.Add(_owner);
            })
            .ReturnsAsync(1);

        // Act
        var changeOwnerResult = await ChatService.ChangeOwnerAsync(_chat.Id, _member.Id, _owner.Id);

        // Assert
        changeOwnerResult.IsSuccess.ShouldBeTrue();

        var matchResult = changeOwnerResult.Match(
            success => true,
            failure => throw new Exception("Should not be success")
        );

        _chat.Owner.ShouldBe(_member);
        _chat.Members.ShouldNotContain(_member);
        _chat.Members.ShouldContain(_owner);

        _member.OwnedChats.ShouldContain(_chat);
        _member.MemberChats.ShouldNotContain(_chat);

        _owner.OwnedChats.ShouldNotContain(_chat);
        _owner.MemberChats.ShouldContain(_chat);

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_chat.Id, _owner.Id));
        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
}