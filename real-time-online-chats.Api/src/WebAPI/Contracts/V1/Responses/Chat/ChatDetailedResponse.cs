using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Contracts.V1.Responses.User;

namespace real_time_online_chats.Server.Contracts.V1.Responses.Chat;

public class ChatDetailedResponse
{
    public required Guid Id { get; set; }
    public required Guid OwnerId { get; set; }
    public required string Title { get; set; }
    public required DateTime CreationTime { get; set; }
    public IEnumerable<MessageChatResponse> Messages { get; set; } = [];
    public IEnumerable<UserChatResponse> Users { get; set; } = [];
}