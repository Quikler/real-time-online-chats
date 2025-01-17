using System;
using real_time_online_chats.Server.Contracts.V1.Responses.Chat;

namespace real_time_online_chats.Server.Contracts.V1.Responses;

public class PaginatedChatResponse
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<GetChatResponse> Chats { get; set; }
}
