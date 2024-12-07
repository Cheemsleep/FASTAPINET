namespace FastApiMvc.Service.Cache
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<bool> RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}
