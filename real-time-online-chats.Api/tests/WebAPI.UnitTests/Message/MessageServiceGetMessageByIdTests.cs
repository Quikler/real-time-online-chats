using AutoFixture;
using MockQueryable.Moq;
using Moq;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Services.Message;
using Shouldly;

namespace WebAPI.UnitTests.Message;

public class MessageServiceGetMessageByIdTests : BaseMessageServiceTests
{
    private readonly UserEntity _user;
    private readonly MessageEntity _message;

    public MessageServiceGetMessageByIdTests()
    {
        _message = Fixture.Create<MessageEntity>();
        _user = Fixture.Create<UserEntity>();
    }

    [Fact]
    public async Task GetMessageByIdAsync_ShouldReturnError_WhenUserDoesntOwnMessage()
    {
        // Arrange
        MessageAuthorizationService
            .Setup(messageAuthorizationService => messageAuthorizationService.UserOwnsMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act
        var confirmEmailResult = await MessageService.GetMessageByIdAsync(_message.Id, _user.Id);

        // Assert
        confirmEmailResult.IsSuccess.ShouldBeFalse();
        var matchResult = confirmEmailResult.Match(
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
        var confirmEmailResult = await MessageService.GetMessageByIdAsync(_message.Id, _user.Id);

        // Assert
        confirmEmailResult.IsSuccess.ShouldBeFalse();
        var matchResult = confirmEmailResult.Match(
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
        const string testContent = "test_content";
        _message.Content = testContent;
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
        var confirmEmailResult = await MessageService.GetMessageByIdAsync(_message.Id, _user.Id);

        // Assert
        confirmEmailResult.IsSuccess.ShouldBeTrue();
        var matchResult = confirmEmailResult.Match(
            messageChatDto => messageChatDto,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.Content.ShouldBe(testContent);
        matchResult.Id.ShouldBe(_message.Id);
        matchResult.User.Id.ShouldBe(_user.Id);

        MessageAuthorizationService.Verify(userManager => userManager.UserOwnsMessageAsync(_message.Id, _user.Id));
    }
}