namespace real_time_online_chats.Server.Contracts.V1.Responses.Chat;

public class ChatInfoResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required Guid OwnerId { get; set; }
    public required DateTime CreationTime { get; set; }
}
