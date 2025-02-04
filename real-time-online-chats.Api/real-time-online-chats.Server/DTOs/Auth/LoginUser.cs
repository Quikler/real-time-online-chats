namespace real_time_online_chats.Server.DTOs.Auth;

public class LoginUserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
