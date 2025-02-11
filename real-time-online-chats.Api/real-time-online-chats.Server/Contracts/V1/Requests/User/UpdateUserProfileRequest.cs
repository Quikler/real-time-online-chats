using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Contracts.V1.Requests.User;

public class UpdateUserProfileRequest
{
    public string? AboutMe { get; set; }
    public ActivityStatus ActivityStatus { get; set; }
    public CasualStatus CasualStatus { get; set; }
    public MoodStatus MoodStatus { get; set; }
    public WorkStatus WorkStatus { get; set; }
    public GamingStatus GamingStatus { get; set; }
    public IFormFile? Avatar { get; set; }
}