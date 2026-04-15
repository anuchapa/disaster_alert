using System;

namespace DisasterAlarm.service.Services.CacheService;

public interface ICacheService
{
    Task SetAsync<T>(string key, T data,int expiryMinute=15 );
    Task<T> GetAsync<T>(string key);
}
