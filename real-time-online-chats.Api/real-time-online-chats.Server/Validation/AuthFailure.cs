
namespace real_time_online_chats.Server.Validation;

public class AuthFailure(IEnumerable<string> errors) : Failure(errors)
{
    public AuthFailure(string error) : this([error]) { }
}