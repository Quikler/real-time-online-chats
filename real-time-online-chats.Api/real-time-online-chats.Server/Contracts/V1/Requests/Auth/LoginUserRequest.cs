using System.ComponentModel.DataAnnotations;

namespace real_time_online_chats.Server.Contracts.V1.Requests.Auth;

public class LoginUserRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}
