using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Message;

namespace real_time_online_chats.Server.Services.Message;

public interface IMessageService
{
    //Task<List<MessageEntity>> GetMessagesAsync();
    Task<Result<MessageChatDto, FailureDto>> GetMessageByIdAsync(Guid messageId, Guid userId);
    Task<Result<MessageChatDto, FailureDto>> CreateMessageAsync(CreateMessageDto createMessageDto);
    Task<Result<MessageChatDto, FailureDto>> UpdateMessageAsync(Guid messageId, UpdateMessageDto updateMessageDto);
    Task<Result<Guid, FailureDto>> DeleteMessageAsync(Guid messageId, Guid userId);
}