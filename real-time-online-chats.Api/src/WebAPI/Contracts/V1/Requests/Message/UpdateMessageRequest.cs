namespace real_time_online_chats.Server.Contracts.V1.Requests.Message;

public class UpdateMessageRequest
{
    public required string Content { get; set; }
    public required Guid ChatId { get; set; }
}