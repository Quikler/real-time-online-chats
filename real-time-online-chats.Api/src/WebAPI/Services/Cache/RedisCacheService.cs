using StackExchange.Redis;
using System.Text.Json;

namespace real_time_online_chats.Server.Services.Cache;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> redisCacheServiceLogger) : IRedisCacheService
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

        var db = connectionMultiplexer.GetDatabase();

        //await db.StringSetAsync(cacheKey, serializedResponse);
        await db.StringSetAsync(cacheKey, serializedResponse, timeToLive);
    }

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        var db = connectionMultiplexer.GetDatabase();
        var cachedResponse = await db.StringGetAsync(cacheKey);
        return cachedResponse;
    }

    public async Task RemoveCachedResponseAsync(string cacheKey)
    {
        var db = connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync(cacheKey);
    }

    public async Task RemoveCachedByTemplateAsync(string template)
    {
        var db = connectionMultiplexer.GetDatabase();

        string script = @"
        local keys = redis.call('keys', ARGV[1])
        if #keys > 0 then
            return redis.call('del', unpack(keys))
        else
            return 0
        end";

        redisCacheServiceLogger.LogInformation("Deleting keys matching: {template}", template);

        // 0 KEYS arguments, 1 ARGV argument
        var result = await db.ScriptEvaluateAsync(
            script,
            keys: [], // no KEYS, template passed as ARGV[1]
            values: [$"{template}*"]
        );

        redisCacheServiceLogger.LogInformation("Deleted {result} keys", result);
    }
}
