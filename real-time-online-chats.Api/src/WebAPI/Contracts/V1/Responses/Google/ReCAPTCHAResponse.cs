using System.Text.Json.Serialization;

namespace real_time_online_chats.Server.Contracts.V1.Responses.Google;

public class ReCAPTCHAResponse
{
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public DateTime ChallengeTimestamp { get; set; }

    public string? Hostname { get; set; }

    [JsonPropertyName("error-codes")]
    public List<string> ErrorCodes { get; set; } = [];
}
