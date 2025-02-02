
namespace real_time_online_chats.Server.Validation;

public class AuthValidationFail(IEnumerable<string> errors) : ValidationFail(errors)
{
    public AuthValidationFail(string error) : this([error]) { }
}