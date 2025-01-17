namespace real_time_online_chats.Server.Contracts.V1.Responses.Auth;

public class AuthFailedResponse
{
    public IEnumerable<string> Errors { get; set; } = [];
}