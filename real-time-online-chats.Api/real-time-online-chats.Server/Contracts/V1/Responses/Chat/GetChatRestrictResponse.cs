using System;
using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Contracts.V1.Responses.Chat;

public class GetChatRestrictResponse
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; }
    public List<GetMessageResponse> Messages { get; set; } = [];
    public List<UserEntity> Members { get; set; } = [];
}
