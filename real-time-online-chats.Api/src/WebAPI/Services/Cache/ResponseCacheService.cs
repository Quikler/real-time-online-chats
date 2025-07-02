using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace real_time_online_chats.Server.Services.Cache;

public class ResponseCacheService(IDistributedCache distributedCache) : IResponseCacheService
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        if (response is null)
        {
            return;
        }

        var serializedResponse = JsonSerializer.Serialize(response, s_jsonSerializerOptions);
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

    public Task RemoveCachedResponseAsync(string cacheKey) => distributedCache.RemoveAsync(cacheKey);
}
