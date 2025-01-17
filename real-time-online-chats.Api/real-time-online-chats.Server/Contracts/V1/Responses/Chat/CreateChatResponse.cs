namespace real_time_online_chats.Server.Contracts.V1.Responses.Chat;

public class CreateChatResponse
{
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
}