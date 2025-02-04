namespace real_time_online_chats.Server.Contracts.V1.Responses.User;

public class UserProfileResponse
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IEnumerable<UserFriendResponse> Friends { get; set; } = [];
    public string? AboutMe { get; internal set; }
    public required string ActivityStatus { get; set; }
    public required string CasualStatus { get; set; }
    public required string GamingStatus { get; set; }
    public required string MoodStatus { get; set; }
    public required string WorkStatus { get; set; }
}