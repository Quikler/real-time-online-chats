namespace real_time_online_chats.Server.Contracts.V1.Responses.Auth;

public class UserResponse
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
