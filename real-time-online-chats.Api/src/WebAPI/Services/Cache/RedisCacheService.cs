using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using StackExchange.Redis;
using System.Text.Json;

namespace real_time_online_chats.Server.Services.Cache;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> redisCacheServiceLogger, IOptions<RedisCacheConfiguration> redisconfigurationOptions) : IRedisCacheService
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly RedisCacheConfiguration _redisCacheConfiguration = redisconfigurationOptions.Value;

    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        if (response is null)
        {
            return;
        }

        var serializedResponse = JsonSerializer.Serialize(response, s_jsonSerializerOptions);

        var db = connectionMultiplexer.GetDatabase();

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
        var server = connectionMultiplexer.GetServer("localhost", 6379); // TODO: Unhardcode this shit. Make separate appsettings.json for Development and Docker.
        //var server = connectionMultiplexer.GetServer(_redisCacheConfiguration.ConnectionString);

        await foreach (RedisKey key in server.KeysAsync(pattern: $"{template}*"))
        {
            await RemoveCachedResponseAsync(key.ToString());
            redisCacheServiceLogger.LogInformation("Key deleted: {key}", key);
        }
    }
}
