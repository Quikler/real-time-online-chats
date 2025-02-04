using real_time_online_chats.Server.Contracts.V1.Responses.User;

namespace real_time_online_chats.Server.Contracts.V1.Responses.Message;

public class MessageChatResponse
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public  UserChatResponse User { get; set; }
}