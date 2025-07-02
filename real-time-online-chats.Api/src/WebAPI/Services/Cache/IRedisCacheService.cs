namespace real_time_online_chats.Server.Services.Cache;

public interface IRedisCacheService : IResponseCacheService
{
    Task RemoveCachedByTemplateAsync(string template);
}
