namespace Application.Interfaces.Cache
{
    public interface ICacheService
    {
        //Task<IEnumerable<T>> SetAllAsync<T>(string key, Func<Task<IEnumerable<T>>> fetchFunction, TimeSpan? expiration = null);
        Task<IEnumerable<T>?> GetAllAsync<T>(string key);
        Task<T> SetAsync<T>(string key, Func<Task<T>> fetchFunction, TimeSpan? expiration = null);
        Task<T?> GetAsync<T>(string key);
        void Clear(string key);
    }
}
