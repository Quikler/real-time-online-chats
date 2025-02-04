namespace real_time_online_chats.Server.DTOs.Chat;

public class CreateChatDto
{
    public required string Title { get; set; }
    public IEnumerable<Guid> UsersIdToAdd { get; set; } = [];
}