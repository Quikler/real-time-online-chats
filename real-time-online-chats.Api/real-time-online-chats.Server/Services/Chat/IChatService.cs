using System.Diagnostics.Eventing.Reader;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Chat;

namespace real_time_online_chats.Server.Services.Chat;

public interface IChatService
{
    // Queries (Read Operations)
    Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetChatsAsync(int pageNumber, int pageSize);
    Task<Result<ChatDetailedDto, FailureDto>> GetChatDetailedByIdAsync(Guid chatId);
    Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetAllOwnedChatsAsync(int pageNumber, int pageSize, Guid userId);

    // Commands (Write Operations)
    Task<Result<ChatPreviewDto, FailureDto>> CreateChatAsync(CreateChatDto createChatDto);
    Task<Result<bool, FailureDto>> UpdateChatAsync(Guid chatId, UpdateChatDto updateChatDto, Guid userId);
    Task<Result<bool, FailureDto>> DeleteChatAsync(Guid chatId, Guid userId);
    Task<Result<bool, FailureDto>> UserJoinChatAsync(Guid chatId, Guid userId);
    Task<Result<bool, FailureDto>> UserLeaveChatAsync(Guid chatId, Guid userId);
    Task<Result<bool, FailureDto>> ChangeOwnerAsync(Guid chatId, Guid newOwnerId);
}
