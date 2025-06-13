namespace real_time_online_chats.Server.Configurations;

public class RedisCacheConfiguration
{
    public bool Enabled { get; set; }
    public required string ConnectionString { get; set; }
}
