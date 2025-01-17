namespace real_time_online_chats.Server.Configurations;

public class JwtConfiguration
{
    public required string SecretKey { get; set; }
    public required TimeSpan TokenLifetime { get; set; }
    public required TimeSpan RefreshTokenLifetime { get; set; }
}