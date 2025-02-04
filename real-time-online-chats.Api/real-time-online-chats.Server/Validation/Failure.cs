namespace real_time_online_chats.Server.Validation;

public class Failure(IEnumerable<string> errors)
{
    public IEnumerable<string> Errors { get; set; } = errors;
}