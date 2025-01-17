namespace real_time_online_chats.Server.Contracts.V1.Responses.Auth;

public class AuthSuccessResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required UserResponse User { get; set; }
}