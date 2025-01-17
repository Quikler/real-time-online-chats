namespace real_time_online_chats.Server.Domain;

public class AuthResult
{
    public bool Succeded { get; set; }
    public IEnumerable<string> Errors { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public UserResult User { get; set; }
}