namespace real_time_online_chats.Server.Contracts.V1.Requests;

public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}
