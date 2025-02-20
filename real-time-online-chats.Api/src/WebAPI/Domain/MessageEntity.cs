namespace real_time_online_chats.Server.Domain;

public class MessageEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public Guid ChatId { get; set; }
    public ChatEntity Chat { get; set; } = null!;
    public required string Content { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
}