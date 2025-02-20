namespace real_time_online_chats.Server.Services.Message;

public interface IMessageAuthorizationService
{

    Task<bool> UserOwnsMessageAsync(Guid messageId, Guid userId);
}