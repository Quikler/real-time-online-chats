using Microsoft.AspNetCore.Mvc;

namespace real_time_online_chats.Server.Contracts.V1.Requests.Chat;

public class ChatsPaginationRequest : PaginationRequest
{
    public int CountOfMessages { get; set; } = 4;
}
