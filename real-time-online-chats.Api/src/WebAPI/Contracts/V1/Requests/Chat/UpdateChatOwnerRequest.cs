namespace real_time_online_chats.Server.Contracts.V1.Requests.Chat;

public class UpdateChatOwnerRequest
{
    public required Guid NewOwnerId { get; set; }
}