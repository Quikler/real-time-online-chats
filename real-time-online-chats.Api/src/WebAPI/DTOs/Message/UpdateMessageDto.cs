namespace real_time_online_chats.Server.DTOs.Message;

public class UpdateMessageDto
{
    public required string Content { get; set; }
    public required Guid UserId { get; set; }
}
