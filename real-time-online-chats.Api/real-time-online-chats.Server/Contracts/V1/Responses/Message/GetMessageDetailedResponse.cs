using real_time_online_chats.Server.Contracts.V1.Responses.Auth;

namespace real_time_online_chats.Server.Contracts.V1.Responses.Message;

public class GetMessageDetailedResponse
{
    public Guid Id { get; set; }
    public UserResponse User { get; set; }
    public string Content { get; set; }
}