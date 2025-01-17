namespace real_time_online_chats.Server.Domain;

public class SignupUser
{
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Phone { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
}