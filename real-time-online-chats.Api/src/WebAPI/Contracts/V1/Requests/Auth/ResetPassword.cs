namespace real_time_online_chats.Server.Contracts.V1.Requests.Auth;

public class ResetPasswordRequest
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}
