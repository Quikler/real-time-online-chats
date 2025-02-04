using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.DTOs.User;

public class UserProfileDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? AboutMe { get; set; }

    public required string ActivityStatus { get; set; }
    public required string CasualStatus { get; set; }
    public required string MoodStatus { get; set; }
    public required string WorkStatus { get; set; }
    public required string GamingStatus { get; set; }

    public IEnumerable<UserFriendDto> Friends { get; set; } = [];
    //public List<ChatEntity> OwnedChats { get; set; }
}