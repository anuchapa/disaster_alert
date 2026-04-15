using System;
using System.Text.Json;
using StackExchange.Redis;

namespace DisasterAlarm.service.Services.CacheService;

public class RedisService : ICacheService
{
    private readonly IDatabase _db;

    public RedisService(string? conn)
    {
        var muxer = ConnectionMultiplexer.Connect(conn);
        _db = muxer.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T data,int expiryMinute=15)
    {
        TimeSpan expiry = TimeSpan.FromMinutes(expiryMinute);
        string json = JsonSerializer.Serialize(data);
        await _db.StringSetAsync(key, json,expiry);
    }
    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNull)  return default;
        return JsonSerializer.Deserialize<T>((string)value!);
    }
}
