using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Chat;
using Shouldly;
using real_time_online_chats.Server.Common;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceUpdateChatTests : BaseChatServiceTests
{
    private readonly UserEntity _user;
    private readonly UpdateChatDto _updateChatDto;
    private readonly ChatEntity _ownedChat;

    public ChatServiceUpdateChatTests()
    {
        _user = Fixture.Create<UserEntity>();
        _updateChatDto = Fixture.Create<UpdateChatDto>();
        _ownedChat = Fixture.Build<ChatEntity>()
            .With(c => c.OwnerId, _user.Id)
            .With(c => c.Owner, _user)
            .Create();
    }

    [Fact]
    public async Task UpdateChatAsync_ShouldReturnError_WhenUserDoesntOwnChat()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var createResult = await ChatService.UpdateChatAsync(chatId, _updateChatDto, _user.Id);

        // Assert
        createResult.IsSuccess.ShouldBeFalse();

        var matchResult = createResult.Match(
            chatPreviewDto => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.Forbidden);
        matchResult.Errors.ShouldContain("User doesn't own this chat");

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(chatId, _user.Id));
    }

    [Fact]
    public async Task CreateChatAsync_ShouldReturnChatPreview_WhenUserOwnsChat()
    {
        // Arrange
        List<ChatEntity> chatEntities = [_ownedChat];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatAuthorizationService
            .Setup(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _user.Id))
            .ReturnsAsync(true);

        ChatRepository
            .Setup(chatRepository => chatRepository.UpdateChatTitleAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var createResult = await ChatService.UpdateChatAsync(_ownedChat.Id, _updateChatDto, _user.Id);

        // Assert
        createResult.IsSuccess.ShouldBeTrue();

        var matchResult = createResult.Match(
            success => true,
            failure => throw new Exception("Should not be failure")
        );

        ChatAuthorizationService.Verify(chatAuthorizationService => chatAuthorizationService.IsUserOwnsChatAsync(_ownedChat.Id, _user.Id));
        ChatRepository.Verify(chatRepository => chatRepository.UpdateChatTitleAsync(_ownedChat.Id, _updateChatDto.Title, It.IsAny<CancellationToken>()));
    }
}