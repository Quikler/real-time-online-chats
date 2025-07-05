namespace real_time_online_chats.Server.DTOs.Auth;

public class ResetPasswordDto
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}
