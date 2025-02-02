namespace real_time_online_chats.Server.Domain;

public class AuthResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public UserResult User { get; set; }
}