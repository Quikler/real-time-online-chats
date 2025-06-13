using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace real_time_online_chats.Server.Services.Cache;

public class ResponseCacheService(IDistributedCache distributedCache) : IResponseCacheService
{
    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        if (response is null)
        {
            return;
        }

        var serializedResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        await distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeToLive,
        });
    }

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await distributedCache.GetStringAsync(cacheKey);
        return cachedResponse;
    }
}
