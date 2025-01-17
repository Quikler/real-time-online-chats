using System;
using System.ComponentModel.DataAnnotations;

namespace real_time_online_chats.Server.Contracts.V1.Requests.Auth;

public class UserLoginRequest
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}
