namespace real_time_online_chats.Server.Contracts.V1.Responses.Message;

public class GetMessageResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
}