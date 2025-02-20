using real_time_online_chats.Server.DTOs.Message;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.DTOs.Chat;

public class ChatDetailedDto
{
    public required Guid Id { get; set; }
    public required Guid OwnerId { get; set; }
    public required string Title { get; set; }
    public required DateTime CreationTime { get; set; }
    public IEnumerable<MessageChatDto> Messages { get; set; } = [];
    public IEnumerable<UserChatDto> Users { get; set; } = [];
}