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
    private readonly UserEntity _owner;
    private readonly CreateChatDto _createChatDto;
    private readonly List<UserEntity> _membersToAdd;

    public ChatServiceCreateChatTests()
    {
        _owner = Fixture.Create<UserEntity>();

        _membersToAdd = [.. Fixture.CreateMany<UserEntity>(3)];

        _createChatDto = Fixture.Build<CreateChatDto>()
            .With(c => c.OwnerId, _owner.Id)
            .With(c => c.UsersIdToAdd, _membersToAdd.Select(u => u.Id))
            .Create();
    }

    [Fact]
    public async Task CreateChatAsync_ShouldReturnError_WhenAddChatAsyncReturnZero()
    {
        // Arrange
        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        List<UserEntity> userEntities = [];
        var userEntitiesDbSetMock = userEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

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
        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        var userEntitiesDbSetMock = _membersToAdd.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

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
        _createChatDto.Title = string.Empty;

        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        var userEntitiesDbSetMock = _membersToAdd.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        DbContextMock
            .Setup(dbContext => dbContext.Users)
            .Returns(userEntitiesDbSetMock.Object);

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
    }
}