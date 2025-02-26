using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;
using real_time_online_chats.Server.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceUserLeaveChatTests : BaseChatServiceTests
{
    private readonly UserEntity _owner;
    private readonly ChatEntity _chat;
    private readonly UserEntity _member;

    public ChatServiceUserLeaveChatTests()
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
    public async Task UserLeaveChatAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        List<UserEntity> userEntities = [];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        // Act
        var leaveResult = await ChatService.UserLeaveChatAsync(_chat.Id, _owner.Id);

        // Assert
        leaveResult.IsSuccess.ShouldBeFalse();

        var matchResult = leaveResult.Match(
            chatPreviewDto => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.NotFound);
        matchResult.Errors.ShouldContain("User not found");
    }

    [Fact]
    public async Task UserLeaveChatAsync_ShouldTransferOwnershipAndReturnUser_WhenUserIsOwnerAndAtLeastOneMemberExist()
    {
        // Arrange
        List<UserEntity> userEntities = [_owner, _member];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var leaveResult = await ChatService.UserLeaveChatAsync(_chat.Id, _owner.Id);

        // Assert
        leaveResult.IsSuccess.ShouldBeTrue();

        var matchResult = leaveResult.Match(
            userChatDto => userChatDto,
            failure => throw new Exception("Should not be failure")
        );

        matchResult.Id.ShouldBe(_owner.Id);
        matchResult.Email.ShouldBe(_owner.Email);
        _owner.OwnedChats.ShouldNotContain(_chat);
        _member.OwnedChats.ShouldContain(_chat);
        _member.MemberChats.ShouldNotContain(_chat);

        _chat.Owner.ShouldBe(_member);
        _chat.OwnerId.ShouldBe(_member.Id);

        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UserLeaveChatAsync_ShouldDeleteChat_WhenUserIsOwnerAndNoMembersExist()
    {
        // Arrange
        _chat.Members = [];

        List<UserEntity> userEntities = [_owner];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        chatEntitiesDbSetMock
            .Setup(chatsDbSet => chatsDbSet.Remove(_chat))
            .Callback(() =>
            {
                _owner.OwnedChats.Remove(_chat);
                chatEntities.Remove(_chat);
            })
            .Returns(It.IsAny<EntityEntry<ChatEntity>>());

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var leaveResult = await ChatService.UserLeaveChatAsync(_chat.Id, _owner.Id);

        // Assert
        leaveResult.IsSuccess.ShouldBeTrue();

        var matchResult = leaveResult.Match(
            userChatDto => userChatDto,
            failure => throw new Exception("Should not be failure")
        );

        matchResult.Id.ShouldBe(_owner.Id);
        matchResult.Email.ShouldBe(_owner.Email);
        _owner.OwnedChats.ShouldNotContain(_chat);
        chatEntities.ShouldNotContain(_chat);

        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
        chatEntitiesDbSetMock.Verify(chatsDbSet => chatsDbSet.Remove(It.Is<ChatEntity>(c => c.Id == _chat.Id)));
    }

    [Fact]
    public async Task UserLeaveChatAsync_ShouldLeaveChat_WhenUserIsMember()
    {
        // Arrange
        _chat.Members = [_member];

        List<UserEntity> userEntities = [_owner, _member];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _chat.Members.Remove(_member);
            })
            .ReturnsAsync(1);

        // Act
        var leaveResult = await ChatService.UserLeaveChatAsync(_chat.Id, _member.Id);

        // Assert
        leaveResult.IsSuccess.ShouldBeTrue();

        var matchResult = leaveResult.Match(
            userChatDto => userChatDto,
            failure => throw new Exception("Should not be failure")
        );

        matchResult.Id.ShouldBe(_member.Id);
        matchResult.Email.ShouldBe(_member.Email);
        _member.MemberChats.ShouldNotContain(_chat);
        _chat.Members.ShouldNotContain(_member);

        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UserLeaveChatAsync_ShouldReturnError_WhenSaveChangesAsync()
    {
        // Arrange
        _chat.Members = [];

        List<UserEntity> userEntities = [_owner, _member];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        List<ChatEntity> chatEntities = [_chat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var leaveResult = await ChatService.UserLeaveChatAsync(_chat.Id, _member.Id);

        // Assert
        leaveResult.IsSuccess.ShouldBeFalse();

        var matchResult = leaveResult.Match(
            userChatDto => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.BadRequest);
        matchResult.Errors.ShouldContain("Cannot leave the chat");

        DbContextMock.Verify(dbContext => dbContext.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
}