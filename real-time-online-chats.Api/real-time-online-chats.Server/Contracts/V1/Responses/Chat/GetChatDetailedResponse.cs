using System;
using real_time_online_chats.Server.Contracts.V1.Responses.Auth;
using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Contracts.V1.Responses.Chat;

public class GetChatDetailedResponse
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Title { get; set; }
    public DateTime CreationTime { get; set; }
    public IEnumerable<GetMessageDetailedResponse> Messages { get; set; } = [];
    public IEnumerable<UserResponse> Users { get; set; } = [];
}