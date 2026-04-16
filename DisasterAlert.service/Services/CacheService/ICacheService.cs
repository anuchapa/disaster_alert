using System;

namespace DisasterAlert.service.Services.CacheService;

public interface ICacheService
{
    Task SetAsync<T>(string key, T data,int expiryMinute=15 );
    Task<T> GetAsync<T>(string key);
    Task<bool> DeletetAsync(string key);
}
