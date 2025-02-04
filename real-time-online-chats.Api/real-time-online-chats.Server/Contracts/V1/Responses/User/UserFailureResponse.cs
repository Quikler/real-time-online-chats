
namespace real_time_online_chats.Server.Contracts.V1.Responses.User;

public class UserFailureResponse(IEnumerable<string> errors) : FailureResponse(errors)
{
}