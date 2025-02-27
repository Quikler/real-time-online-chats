using System.ComponentModel.DataAnnotations;

namespace real_time_online_chats.Server.Contracts.V1.Requests.Auth;

public class SignupUserRequest
{
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public required string Phone { get; set; }

    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public bool RememberMe { get; set; }
}