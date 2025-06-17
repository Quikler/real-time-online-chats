using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Message;

namespace real_time_online_chats.Server.Services.Message;

public interface IChatMessageService
{
    Task<List<MessageChatDto>> GetAllMessagesByChatIdAsync(Guid chatId);
    Task<ResultDto<MessageChatDto>> GetMessageByIdAsync(Guid chatId, Guid messageId, Guid userId);
    Task<ResultDto<MessageChatDto>> CreateMessageAsync(Guid chatId, CreateMessageDto createMessageDto);
    Task<ResultDto<MessageChatDto>> UpdateMessageContentAsync(Guid chatId, Guid messageId, UpdateMessageDto updateMessageDto);
    Task<Result<Guid, FailureDto>> DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId);
}
