namespace real_time_online_chats.Server.DTOs.Chat;

public class ChatInfoDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required Guid OwnerId { get; set; }
    public required DateTime CreationTime { get; set; }
}
