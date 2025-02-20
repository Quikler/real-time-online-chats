using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.DTOs.Auth;

public class AuthSuccessDto
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required UserGlobalDto User { get; set; }
}