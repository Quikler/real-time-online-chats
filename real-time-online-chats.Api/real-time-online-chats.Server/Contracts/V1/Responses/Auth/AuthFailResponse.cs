
namespace real_time_online_chats.Server.Contracts.V1.Responses.Auth;

public class AuthFailResponse(IEnumerable<string> errors) : FailResponse(errors)
{
    public AuthFailResponse(string error) : this([error]) { }
}