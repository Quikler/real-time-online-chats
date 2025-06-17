using Moq;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Services.Chat;
using real_time_online_chats.Server.Services.Message;

namespace WebAPI.UnitTests.Message;

public class BaseMessageServiceTests : BaseUnitTests
{
    protected virtual Mock<AppDbContext> DbContextMock { get; }
    protected virtual Mock<MessageAuthorizationService> MessageAuthorizationService { get; }
    protected virtual Mock<ChatAuthorizationService> ChatAuthorizationService { get; }

    protected virtual ChatMessageService MessageService { get; }

    public BaseMessageServiceTests()
    {
        DbContextMock = new Mock<AppDbContext>();

        MessageAuthorizationService = new Mock<MessageAuthorizationService>(DbContextMock.Object);
        ChatAuthorizationService = new Mock<ChatAuthorizationService>(DbContextMock.Object);

        MessageService = new ChatMessageService(DbContextMock.Object, MessageAuthorizationService.Object, ChatAuthorizationService.Object);
    }
}
