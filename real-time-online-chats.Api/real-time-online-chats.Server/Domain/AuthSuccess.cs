namespace real_time_online_chats.Server.Domain;

public class AuthSuccess
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required UserSuccess User { get; set; }
}