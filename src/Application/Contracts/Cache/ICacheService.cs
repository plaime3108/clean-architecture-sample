namespace Application.Contracts.Cache
{
    public interface ICacheService<T>
    {
        Task<IEnumerable<T>> GetOrSetAsync(string key, Func<Task<IEnumerable<T>>> fetchFunction, TimeSpan? expiration = null);
        Task RefreshAsync(string key, Func<Task<IEnumerable<T>>> fetchFunction, TimeSpan? expiration = null);
        Task<IEnumerable<T>?> TryGetAsync(string key);
        void Clear(string key);
    }
}
