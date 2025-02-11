using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.DTOs.User;

public class UpdateUserProfileDto
{
    public string? AboutMe { get; set; }

    public required ActivityStatus ActivityStatus { get; set; }
    public required CasualStatus CasualStatus { get; set; }
    public required MoodStatus MoodStatus { get; set; }
    public required WorkStatus WorkStatus { get; set; }
    public required GamingStatus GamingStatus { get; set; }

    public Stream? AvatarStream { get; set; }
}