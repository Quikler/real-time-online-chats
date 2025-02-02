namespace real_time_online_chats.Server.Contracts.V1.Responses;

public class FailResponse
{
    public IEnumerable<string> Errors { get; set; } = [];

    public FailResponse(IEnumerable<string> errors) => Errors = errors;
}