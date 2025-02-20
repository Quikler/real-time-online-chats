namespace real_time_online_chats.Server.Domain;

public class ChatEntity : BaseEntity
{
    public required Guid OwnerId { get; set; }
    public UserEntity Owner { get; set; } = null!;

    public List<UserEntity> Members { get; set; } = [];
    
    public required string Title { get; set; }
    public List<MessageEntity> Messages { get; set; } = [];

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
}