using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Services.Chat;

public interface IChatService
{
    // Queries (Read Operations)
    Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetChatsAsync(int pageNumber, int pageSize, string filter = "");
    Task<Result<ChatDetailedDto, FailureDto>> GetChatDetailedByIdAsync(Guid chatId);
    Task<Result<ChatPreviewDto, FailureDto>> GetChatPreviewByIdAsync(Guid chatId);
    Task<Result<ChatInfoDto, FailureDto>> GetChatInfo(Guid chatId);

    // Commands (Write Operations)
    Task<Result<ChatPreviewDto, FailureDto>> CreateChatAsync(CreateChatDto createChatDto);
    Task<Result<bool, FailureDto>> UpdateChatTitleAsync(Guid chatId, UpdateChatDto updateChatDto, Guid userId);
    Task<Result<bool, FailureDto>> DeleteChatAsync(Guid chatId, Guid userId);
    Task<Result<(UserChatDto user, bool isAlreadyInChat), FailureDto>> UserJoinChatAsync(Guid chatId, Guid userId);
    Task<Result<UserChatDto, FailureDto>> UserLeaveChatAsync(Guid chatId, Guid userId);
    Task<Result<bool, FailureDto>> KickMemberAsync(Guid chatId, Guid userId, Guid currentUserId);
    Task<Result<bool, FailureDto>> UpdateOwnerAsync(Guid chatId, Guid newOwnerId, Guid userId);
}
