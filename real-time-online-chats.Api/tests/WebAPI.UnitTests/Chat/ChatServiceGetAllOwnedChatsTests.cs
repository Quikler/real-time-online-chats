using AutoFixture;
using MockQueryable.Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceGetAllOwnedChatsTests : BaseChatServiceTests
{
    private readonly UserEntity _user;

    public ChatServiceGetAllOwnedChatsTests()
    {
        _user = Fixture.Create<UserEntity>();
    }

    [Fact]
    public async Task GetAllOwnedChatsAsync_ShouldReturnEmptyPagination_WhenNoChatsOfUser()
    {
        // Arrange
        List<ChatEntity> chatEntities = [.. Fixture.Build<ChatEntity>()
            .Without(c => c.OwnerId)
            .Without(c => c.Owner)
            .CreateMany(3)];

        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbCotnext => dbCotnext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetAllOwnedChatsAsync(1, 1, _user.Id);

        // Assert
        chatsResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatsResult.Match(
            paginationResult => paginationResult,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.Items.ShouldBeEmpty();
        matchResult.PageNumber.ShouldBe(1);
        matchResult.PageSize.ShouldBe(1);
        matchResult.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetAllOwnedChatsAsync_ShouldReturnPagination_WhenChatsOfUser()
    {
        // Arrange
        List<ChatEntity> chatEntities = [.. Fixture.Build<ChatEntity>()
            .With(c => c.OwnerId, _user.Id)
            .With(c => c.Owner, _user)
            .CreateMany(3)];

        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbCotnext => dbCotnext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetAllOwnedChatsAsync(1, chatEntities.Count, _user.Id);

        // Assert
        chatsResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatsResult.Match(
            paginationResult => paginationResult,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.Items.Count().ShouldBe(chatEntities.Count);
        matchResult.PageNumber.ShouldBe(1);
        matchResult.PageSize.ShouldBe(chatEntities.Count);
        matchResult.TotalCount.ShouldBe(chatEntities.Count);
    }
}