namespace real_time_online_chats.Server.DTOs;

public class FailureDto(IEnumerable<string> errors)
{
    public IEnumerable<string> Errors { get; set; } = errors;

    public FailureDto(string error) : this([error]) { }
}