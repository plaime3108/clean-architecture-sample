namespace Application.Contracts.Cache
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> SetAsync<T>(string key, Func<Task<IEnumerable<T>>> fetchFunction, TimeSpan? expiration = null);
        Task<IEnumerable<T>?> GetAsync<T>(string key);
        void Clear(string key);
    }
}
