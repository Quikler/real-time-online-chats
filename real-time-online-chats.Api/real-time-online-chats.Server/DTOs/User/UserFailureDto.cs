
namespace real_time_online_chats.Server.DTOs.User;

public class UserFailureDto(IEnumerable<string> errors) : FailureDto(errors)
{
    public UserFailureDto(string error) : this([error]) { }
}