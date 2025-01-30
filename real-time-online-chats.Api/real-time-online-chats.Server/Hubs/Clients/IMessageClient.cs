using real_time_online_chats.Server.Contracts.V1.Responses.Message;

namespace real_time_online_chats.Server.Hubs.Clients;

public interface IMessageClient
{
    Task SendMessage(GetMessageResponse response);
    Task LeaveChat(Guid userId);
    Task JoinChat(Guid userId);
}