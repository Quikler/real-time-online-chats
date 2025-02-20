namespace real_time_online_chats.Server.DTOs;

public class PaginationDto<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = [];
}