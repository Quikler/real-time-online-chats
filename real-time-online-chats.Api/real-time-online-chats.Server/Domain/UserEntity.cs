using Microsoft.AspNetCore.Identity;

namespace real_time_online_chats.Server.Domain;

public class UserEntity : IdentityUser<Guid>
{
    public new string Email
    {
        get => base.Email ?? "";
        set => base.Email = value;
    }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? AboutMe { get; set; }

    public ActivityStatus ActivityStatus { get; set; }
    public CasualStatus CasualStatus { get; set; }
    public MoodStatus MoodStatus { get; set; }
    public WorkStatus WorkStatus { get; set; }
    public GamingStatus GamingStatus { get; set; }

    public List<UserEntity> Friends { get; set; } = [];
    public List<ChatEntity> OwnedChats { get; set; } = [];
    public List<ChatEntity> MemberChats { get; set; } = [];
    public List<MessageEntity> Messages { get; set; } = [];
}