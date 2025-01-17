namespace real_time_online_chats.Server.Contracts.V1.Requests.Message;

public class CreateMessageRequest
{
    public string Content { get; set; }
    public string ContentType { get; set; }
    public Guid ChatId { get; set; }
}