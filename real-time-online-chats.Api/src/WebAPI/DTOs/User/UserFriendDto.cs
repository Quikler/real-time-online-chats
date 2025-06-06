namespace real_time_online_chats.Server.DTOs.User;

public class UserFriendDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}