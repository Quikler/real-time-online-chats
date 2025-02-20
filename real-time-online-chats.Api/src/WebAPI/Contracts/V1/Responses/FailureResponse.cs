namespace real_time_online_chats.Server.Contracts.V1.Responses;

public class FailureResponse(IEnumerable<string> errors)
{
    public IEnumerable<string> Errors { get; set; } = errors;
}