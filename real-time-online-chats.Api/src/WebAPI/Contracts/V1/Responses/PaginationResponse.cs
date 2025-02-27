namespace real_time_online_chats.Server.Contracts.V1.Responses;

public class PaginationResponse<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = [];
}
