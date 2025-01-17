using Microsoft.AspNetCore.Identity;

namespace real_time_online_chats.Server.Domain;

public class UserEntity : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public List<UserEntity> Friends { get; set; } = [];
    public List<ChatEntity> OwnedChats { get; set; } = [];
    public List<ChatEntity> MemberChats { get; set; } = [];
    public List<MessageEntity> Messages { get; set; } = [];
}