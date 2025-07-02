namespace real_time_online_chats.Server.Contracts.HealthCheck;

public class HealthCheckResponse
{
    public string? Status { get; set; }
    public IEnumerable<HealthCheck> Checks { get; set; } = [];
    public TimeSpan Duration { get; set; }
}
