using AutoFixture;
using MockQueryable.Moq;
using real_time_online_chats.Server.Domain;
using Shouldly;

namespace WebAPI.UnitTests.Chat;

public class ChatServiceGetChatsTests : BaseChatServiceTests
{
    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(-1, 5)]
    public async Task GetChatsAsync_ShouldReturnError_WhenInvalidPageOrSize(int pageNumber, int pageSize)
    {
        // Arrange

        // Act
        var chatsResult = await ChatService.GetChatsAsync(pageNumber, pageSize);

        // Assert
        chatsResult.IsSuccess.ShouldBeFalse();

        var matchResult = chatsResult.Match(
            paginationResult => [],
            failure => failure.Errors
        );

        matchResult.ShouldContain("Invalid page size or page number.");
    }

    [Fact]
    public async Task GetChatsAsync_ShouldReturnEmptyPagination_WhenNoChats()
    {
        // Arrange
        List<ChatEntity> chatEntities = [];
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbCotnext => dbCotnext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetChatsAsync(1, 1);

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

    public static IEnumerable<object[]> Chats => [
        [Fixture.CreateMany<ChatEntity>(3)],
        [Fixture.CreateMany<ChatEntity>(4)],
        [Fixture.CreateMany<ChatEntity>(5)],
    ];

    [Theory]
    [MemberData(nameof(Chats))]
    public async Task GetChatsAsync_ShouldReturnPagination_WhenPageSizeIsSameAsChats(IEnumerable<ChatEntity> chatEntities)
    {
        // Arrange
        var count = chatEntities.ToArray().Length;
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbCotnext => dbCotnext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetChatsAsync(1, count);

        // Assert
        chatsResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatsResult.Match(
            paginationResult => paginationResult,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.Items.ToArray().Length.ShouldBe(count);
        matchResult.PageNumber.ShouldBe(1);
        matchResult.PageSize.ShouldBe(count);
        matchResult.TotalCount.ShouldBe(count);
    }

    [Theory]
    [MemberData(nameof(Chats))]
    public async Task GetChatAsync_ShouldReturnPagination_WhenPageSizeBiggerThanChats(IEnumerable<ChatEntity> chatEntities)
    {
        // Arrange
        var count = chatEntities.ToArray().Length;
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbCotnext => dbCotnext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetChatsAsync(1, count + 5);

        // Assert
        chatsResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatsResult.Match(
            paginationResult => paginationResult,
            failure => throw new Exception("Should not be failure.")
        );

        matchResult.Items.ToArray().Length.ShouldBe(count);
        matchResult.PageNumber.ShouldBe(1);
        matchResult.PageSize.ShouldBe(count + 5);
        matchResult.TotalCount.ShouldBe(count);
    }

    public static IEnumerable<object[]> ChatsFrom4 => [
        [Fixture.CreateMany<ChatEntity>(4)],
        [Fixture.CreateMany<ChatEntity>(5)],
        [Fixture.CreateMany<ChatEntity>(6)],
    ];

    [Theory]
    [MemberData(nameof(ChatsFrom4))]
    public async Task GetChatAsync_ShouldReturnPagination_WhenPageSizeFirst(IEnumerable<ChatEntity> chatEntities)
    {
        // Arrange
        var chatEntitiesArr = chatEntities.ToArray();

        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetChatsAsync(1, 2);

        // Assert
        chatsResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatsResult.Match(
            paginationResult => paginationResult,
            failure => throw new Exception("Should not be failure.")
        );

        var items = matchResult.Items.ToArray();

        items.Length.ShouldBe(2);
        items[0].Title.ShouldBe(chatEntitiesArr[0].Title);
        items[1].Title.ShouldBe(chatEntitiesArr[1].Title);

        matchResult.PageNumber.ShouldBe(1);
        matchResult.PageSize.ShouldBe(2);
        matchResult.TotalCount.ShouldBe(chatEntitiesArr.Length);
    }

    [Theory]
    [MemberData(nameof(ChatsFrom4))]
    public async Task GetChatAsync_ShouldReturnPagination_WhenPageNumberIsNext(IEnumerable<ChatEntity> chatEntities)
    {
        // Arrange
        var chatEntitiesArr = chatEntities.ToArray();

        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetChatsAsync(2, 2);

        // Assert
        chatsResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatsResult.Match(
            paginationResult => paginationResult,
            failure => throw new Exception("Should not be failure.")
        );

        var items = matchResult.Items.ToArray();

        items.Length.ShouldBe(2);
        items[0].Title.ShouldBe(chatEntitiesArr[2].Title);
        items[1].Title.ShouldBe(chatEntitiesArr[3].Title);

        matchResult.PageNumber.ShouldBe(2);
        matchResult.PageSize.ShouldBe(2);
        matchResult.TotalCount.ShouldBe(chatEntitiesArr.Length);
    }

    [Theory]
    [MemberData(nameof(Chats))]
    public async Task GetChatsAsync_ShouldReturnEmptyPagination_WhenPageNumberBiggerThanTotalPages(IEnumerable<ChatEntity> chatEntities)
    {
        // Arrange
        var count = chatEntities.ToArray().Length;
        var chatEntitiesDbSetMock = chatEntities.BuildMockDbSet();

        DbContextMock
            .Setup(dbContext => dbContext.Chats)
            .Returns(chatEntitiesDbSetMock.Object);

        // Act
        var chatsResult = await ChatService.GetChatsAsync(count + 6, count);

        // Assert
        chatsResult.IsSuccess.ShouldBeTrue();

        var matchResult = chatsResult.Match(
            paginationResult => paginationResult,
            failure => throw new Exception("Should not be failure.")
        );

        var items = matchResult.Items.ToArray();

        items.ShouldBeEmpty();
        matchResult.PageNumber.ShouldBe(count + 6);
        matchResult.PageSize.ShouldBe(count);
        matchResult.TotalCount.ShouldBe(count);
    }
}