namespace real_time_online_chats.Server.DTOs.Auth
{
    public class EmailConfirmDto
    {
        public required Guid UserId { get; set; }
        public required string Token { get; set; }
    }
}