using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Services.Message;

public interface IMessageService
{
    Task<List<MessageEntity>> GetMessagesAsync();
    Task<MessageEntity?> GetMessageByIdAsync(Guid messageId);
    Task<bool> CreateMessageAsync(MessageEntity message);
    Task<bool> UpdateMessageAsync(MessageEntity message);
    Task<bool> DeleteMessageAsync(Guid messageId);
    Task<bool> UserOwnsMessageAsync(Guid messageId, Guid userId);
}