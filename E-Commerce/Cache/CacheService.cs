using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace E_Commerce.Cache;

public class CacheService(IDistributedCache cache, ILogger<CacheService> logger)
{
    public async Task SetCacheAsync<T>(string key, T data, TimeSpan? expiration = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                // Fallback to a default of 5 minutes if no custom expiration is passed
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            var serializedData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(key, serializedData, options);

            logger.LogDebug("Successfully wrote key '{Key}' to cache.", key);
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Error writing key '{Key}' into Cache.", key);
        }
    }

    public async Task<T?> GetFromCacheAsync<T>(string key)
    {
        try
        {
            logger.LogInformation("Checking Cache for key '{Key}'", key);
            var cachedData = await cache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cachedData))
            {
                logger.LogInformation("Cache Hit for key '{Key}'", key);
                return JsonSerializer.Deserialize<T>(cachedData);
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Cache read failed for key '{Key}', falling back.", key);
        }

        return default;
    }
}