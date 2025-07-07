using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Message;

public class MessageServiceGetMessageByIdTests : BaseMessageServiceTests
{
    private readonly UserEntity _user;
    private readonly ChatEntity _chat;
    private readonly MessageEntity _message;

    public MessageServiceGetMessageByIdTests()
    {
        _user = Fixture.Create<UserEntity>();
        _chat = Fixture.Create<ChatEntity>();
        _message = Fixture.Build<MessageEntity>()
            .With(m => m.ChatId, _chat.Id)
            .Create();
    }

    [Fact]
    public async Task GetMessageByIdAsync_ShouldReturnError_WhenUserDoesntOwnMessage()
    {
        // Arrange
        MessageAuthorizationService
            .Setup(messageAuthorizationService => messageAuthorizationService.UserOwnsMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var message = await MessageService.GetMessageByIdAsync(_chat.Id, _message.Id, _user.Id);

        // Assert
        message.IsSuccess.ShouldBeFalse();
        var matchResult = message.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("User doesn't own this message.");

        MessageAuthorizationService.Verify(userManager => userManager.UserOwnsMessageAsync(_message.Id, _user.Id));
    }

    [Fact]
    public async Task GetMessageByIdAsync_ShouldReturnError_WhenMessageNotFound()
    {
        // Arrange
        List<MessageEntity> messages = [];
        var messagesDbSetMock = messages.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Messages)
            .Returns(messagesDbSetMock.Object);

        MessageAuthorizationService
            .Setup(messageAuthorizationService => messageAuthorizationService.UserOwnsMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var message = await MessageService.GetMessageByIdAsync(_chat.Id, _message.Id, _user.Id);

        // Assert
        message.IsSuccess.ShouldBeFalse();
        var matchResult = message.Match(
            success => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Message not found.");

        MessageAuthorizationService.Verify(userManager => userManager.UserOwnsMessageAsync(_message.Id, _user.Id));
    }

    [Fact]
    public async Task GetMessageByIdAsync_ShouldReturnMessage_WhenEveryCheckPasses()
    {
        // Arrange
        _message.User = _user;
        _message.UserId = _user.Id;

        List<MessageEntity> messages = [_message];
        var messagesDbSetMock = messages.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Messages)
            .Returns(messagesDbSetMock.Object);

        MessageAuthorizationService
            .Setup(messageAuthorizationService => messageAuthorizationService.UserOwnsMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var message = await MessageService.GetMessageByIdAsync(_chat.Id, _message.Id, _user.Id);

        // Assert
        message.IsSuccess.ShouldBeTrue();
        var matchResult = message.Match(
            messageChatDto => messageChatDto,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.Content.ShouldBe(_message.Content);
        matchResult.Id.ShouldBe(_message.Id);
        matchResult.User.Id.ShouldBe(_user.Id);

        MessageAuthorizationService.Verify(userManager => userManager.UserOwnsMessageAsync(_message.Id, _user.Id));
    }
}
