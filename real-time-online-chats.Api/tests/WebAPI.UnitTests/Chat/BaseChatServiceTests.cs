using Moq;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Repositories.Chat;
using real_time_online_chats.Server.Services.Chat;

namespace WebAPI.UnitTests.Chat;

public class BaseChatServiceTests : BaseUnitTests
{
    protected virtual Mock<AppDbContext> DbContextMock { get; }
    protected virtual Mock<ChatAuthorizationService> ChatAuthorizationService { get; }
    protected virtual Mock<ChatRepository> ChatRepository { get; }

    protected virtual ChatService ChatService { get; }

    public BaseChatServiceTests()
    {
        DbContextMock = new Mock<AppDbContext>();

        ChatAuthorizationService = new Mock<ChatAuthorizationService>(DbContextMock.Object);
        ChatRepository = new Mock<ChatRepository>(DbContextMock.Object);

        ChatService = new ChatService(DbContextMock.Object, ChatAuthorizationService.Object, ChatRepository.Object);
    }
}