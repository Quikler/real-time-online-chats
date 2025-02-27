namespace real_time_online_chats.Server.Contracts.V1.Requests.Auth;

public class ConfirmEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
}