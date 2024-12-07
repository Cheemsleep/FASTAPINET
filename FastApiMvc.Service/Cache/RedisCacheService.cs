using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace FastApiMvc.Service.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _logger = logger;
            _db = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                if (!value.HasValue)
                    return default;

                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from Redis for key: {Key}", key);
                return default;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.StringSetAsync(key, serializedValue, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in Redis for key: {Key}", key);
                return false;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing key from Redis: {Key}", key);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking key existence in Redis: {Key}", key);
                return false;
            }
        }
    }
}
