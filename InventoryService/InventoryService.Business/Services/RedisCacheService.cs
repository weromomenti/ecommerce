using InventoryService.Business.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace InventoryService.Business.Services
{
    public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
    {
        private readonly IDatabase database = redis.GetDatabase();

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }
            var value = database.StringGet(key);
            return value.IsNullOrEmpty ? Task.FromResult<T?>(null) : Task.FromResult(JsonSerializer.Deserialize<T>(value));
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            var jsonValue = JsonSerializer.Serialize(value);
            await database.StringSetAsync(key, jsonValue, expiration);
        }

        public async Task RemoveAsync(string key)
        {
            await database.KeyDeleteAsync(key);
        }
    }
}
