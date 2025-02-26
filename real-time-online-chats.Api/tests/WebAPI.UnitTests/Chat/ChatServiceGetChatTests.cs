using AutoFixture;
using MockQueryable.Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceGetChatTests : BaseChatServiceTests
{
    private readonly ChatEntity _chat;

    public ChatServiceGetChatTests()
    {
        _chat = Fixture.Create<ChatEntity>();
    }

    [Fact]
    public async Task GetChatDetailedByIdAsync_ShouldReturnError_WhenChatNotFound()
    {
        // Arrange
        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatResult = await ChatService.GetChatDetailedByIdAsync(_chat.Id);

        // Assert
        chatResult.IsSuccess.ShouldBeFalse();

        var matchResult = chatResult.Match(
            paginationResult => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Chat not found");
    }

    [Fact]
    public async Task GetChatDetailedByIdAsync_ShouldReturnChat_WhenChatFound()
    {
        // Arrange
        List<ChatEntity> chatEntities = [_chat,];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatResult = await ChatService.GetChatDetailedByIdAsync(_chat.Id);

        // Assert
        chatResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatResult.Match(
            chatDetailedDto => chatDetailedDto,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.CreationTime.ShouldBe(_chat.CreationTime);
        matchResult.Id.ShouldBe(_chat.Id);
        matchResult.OwnerId.ShouldBe(_chat.OwnerId);
        matchResult.Title.ShouldBe(_chat.Title);
    }
}