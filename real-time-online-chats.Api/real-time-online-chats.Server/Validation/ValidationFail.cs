namespace real_time_online_chats.Server.Validation;

public class ValidationFail
{
    public IEnumerable<string> Errors { get; set; } = [];

    public ValidationFail(IEnumerable<string> errors) => Errors = errors;
}