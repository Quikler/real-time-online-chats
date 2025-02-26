using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Chat;
using Shouldly;
using real_time_online_chats.Server.Common;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceCreateChatTests : BaseChatServiceTests
{
    private readonly UserEntity _user;
    private readonly CreateChatDto _createChatDto;

    public ChatServiceCreateChatTests()
    {
        _user = Fixture.Create<UserEntity>();
        _createChatDto = Fixture.Build<CreateChatDto>()
            .With(c => c.OwnerId, _user.Id)
            .Create();
    }

    [Fact]
    public async Task CreateChatAsync_ShouldReturnError_WhenAddChatAsyncReturnZero()
    {
        // Arrange
        List<ChatEntity> chatEntities = [.. Fixture.CreateMany<ChatEntity>()];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatRepository
            .Setup(chatRepository => chatRepository.AddChatAsync(It.IsAny<ChatEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var createResult = await ChatService.CreateChatAsync(_createChatDto);

        // Assert
        createResult.IsSuccess.ShouldBeFalse();

        var matchResult = createResult.Match(
            chatPreviewDto => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.BadRequest);
        matchResult.Errors.ShouldContain("Cannot create chat");

        ChatRepository.Verify(chatRepository => chatRepository.AddChatAsync(
            It.Is<ChatEntity>(chat => chat.Title == _createChatDto.Title && chat.OwnerId == _createChatDto.OwnerId),
            It.IsAny<CancellationToken>()
        ));
    }

    [Fact]
    public async Task CreateChatAsync_ShouldReturnChatPreview_WhenSuccess()
    {
        // Arrange
        List<ChatEntity> chatEntities = [.. Fixture.CreateMany<ChatEntity>()];

        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        ChatRepository
            .Setup(chatRepository => chatRepository.AddChatAsync(It.IsAny<ChatEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var createResult = await ChatService.CreateChatAsync(_createChatDto);

        // Assert
        createResult.IsSuccess.ShouldBeTrue();

        var matchResult = createResult.Match(
            chatPreviewDto => chatPreviewDto,
            failure => throw new Exception("Should not be failure")
        );

        matchResult.Title.ShouldBe(_createChatDto.Title);

        ChatRepository.Verify(chatRepository => chatRepository.AddChatAsync(
            It.Is<ChatEntity>(chat => chat.Title == _createChatDto.Title && chat.OwnerId == _createChatDto.OwnerId),
            It.IsAny<CancellationToken>()
        ));
    }

    [Fact]
    public async Task CreateChatAsync_ShouldReturnError_WhenTitleIsEmpty()
    {
        // Arrange
        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        var invalidChatDto = Fixture.Build<CreateChatDto>()
            .With(c => c.Title, string.Empty) // Empty title
            .Create();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var createResult = await ChatService.CreateChatAsync(invalidChatDto);

        // Assert
        createResult.IsSuccess.ShouldBeFalse();

        var matchResult = createResult.Match(
            chatPreviewDto => throw new Exception("Should not be success"),
            failure => failure
        );

        matchResult.FailureCode.ShouldBe(FailureCode.BadRequest);
        matchResult.Errors.ShouldContain("Cannot create chat");
    }
}