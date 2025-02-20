namespace real_time_online_chats.Server.Contracts.V1.Requests.Message;

public class CreateMessageRequest
{
    public required Guid ChatId { get; set; }
    public required string Content { get; set; }
}