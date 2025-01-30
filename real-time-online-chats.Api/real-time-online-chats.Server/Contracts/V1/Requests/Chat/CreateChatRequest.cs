namespace real_time_online_chats.Server.Contracts.V1.Requests.Chat;

public class CreateChatRequest
{
    public required string Title { get; set; }
    public IEnumerable<Guid> UsersIds { get; set; } = [];
}