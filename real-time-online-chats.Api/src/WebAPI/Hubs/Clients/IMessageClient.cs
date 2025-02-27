using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Contracts.V1.Responses.User;

namespace real_time_online_chats.Server.Hubs.Clients;

public interface IMessageClient
{
    Task SendMessage(MessageChatResponse response);
    Task UpdateMessage(MessageChatResponse response);
    Task DeleteMessage(Guid messageId);
    Task LeaveChat(UserChatResponse response);
    Task JoinChat(UserChatResponse response);
    Task KickMember(Guid memberId);
    Task ChangeOwner(Guid oldOwner, Guid newOwner);
}