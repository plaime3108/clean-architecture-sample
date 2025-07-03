using Application.Interfaces.Cache;
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
        //public async Task<IEnumerable<T>> SetAllAsync<T>(string key, Func<Task<IEnumerable<T>>> fetchFunction, TimeSpan? expiration = null)
        //{
        //    var data = await fetchFunction();
        //    var serializedData = JsonSerializer.Serialize(data);
        //    var options = new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
        //    };
        //    await _distributedCache.SetStringAsync(key, serializedData, options);
        //    return data;
        //}
        public async Task<IEnumerable<T>?> GetAllAsync<T>(string key)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            return string.IsNullOrEmpty(cachedData) ? null : JsonSerializer.Deserialize<IEnumerable<T>>(cachedData);
        }

        public async Task<T> SetAsync<T>(string key, Func<Task<T>> fetchFunction, TimeSpan? expiration = null)
        {
            var data = await fetchFunction();
            var serializedData = JsonSerializer.Serialize(data);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
            };
            await _distributedCache.SetStringAsync(key, serializedData, options);
            return data;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            return string.IsNullOrEmpty(cachedData) ? default : JsonSerializer.Deserialize<T>(cachedData);
        }
    }
}
