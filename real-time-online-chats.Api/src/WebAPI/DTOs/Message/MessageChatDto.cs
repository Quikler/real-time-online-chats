using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.DTOs.Message;

public class MessageChatDto
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public required UserChatDto User { get; set; }
}