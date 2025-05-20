using Application.Common.Enums;
using Application.Contracts.Cache;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Services.Cache
{
    public class RedisCacheService(IDistributedCache distributedCache) : ICacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;

        public void Clear(string key)
        {
            _distributedCache.Remove(key);
        }

        public async Task<IEnumerable<T>> SetAsync<T>(string key, Func<Task<IEnumerable<T>>> fetchFunction, TimeSpan? expiration = null)
        {
            //var cachedData = await _distributedCache.GetStringAsync(key);
            //if (!string.IsNullOrEmpty(cachedData))
            //    return JsonSerializer.Deserialize<IEnumerable<T>>(cachedData)!;
            var data = await fetchFunction();
            var serializedData = JsonSerializer.Serialize(data);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
            };
            await _distributedCache.SetStringAsync(key, serializedData, options);
            return data;
        }

        //public async Task RefreshAsync<T>(string key, Func<Task<IEnumerable<T>>> fetchFunction, TimeSpan? expiration = null)
        //{
        //    var data = await fetchFunction();
        //    var serializedData = JsonSerializer.Serialize(data);
        //    var options = new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
        //    };
        //    await _distributedCache.SetStringAsync(key, serializedData, options);
        //}

        public async Task<IEnumerable<T>?> GetAsync<T>(string key)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            return string.IsNullOrEmpty(cachedData) ? null : JsonSerializer.Deserialize<IEnumerable<T>>(cachedData);
        }
    }
}
