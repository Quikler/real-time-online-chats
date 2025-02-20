namespace real_time_online_chats.Server.DTOs.Message;

public class CreateMessageDto
{
    public required string Content { get; set; }
    public required Guid ChatId { get; set; }
    public required Guid UserId { get; set; }
}