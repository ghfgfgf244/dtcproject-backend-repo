using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace dtc.API.Utilities
{
    internal static class DistributedRequestRateLimiter
    {
        public static async Task<bool> IsLimitedAsync(
            IDistributedCache cache,
            string key,
            int limit,
            TimeSpan window)
        {
            var now = DateTimeOffset.UtcNow;
            var snapshot = await ReadSnapshotAsync(cache, key);

            if (snapshot == null || snapshot.WindowEnd <= now)
            {
                snapshot = new RateLimitSnapshot
                {
                    Count = 1,
                    WindowEnd = now.Add(window)
                };

                await WriteSnapshotAsync(cache, key, snapshot, window);
                return false;
            }

            if (snapshot.Count >= limit)
            {
                return true;
            }

            snapshot.Count += 1;
            var remaining = snapshot.WindowEnd - now;
            await WriteSnapshotAsync(cache, key, snapshot, remaining > TimeSpan.Zero ? remaining : window);
            return false;
        }

        private static async Task<RateLimitSnapshot?> ReadSnapshotAsync(IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<RateLimitSnapshot>(json);
        }

        private static Task WriteSnapshotAsync(
            IDistributedCache cache,
            string key,
            RateLimitSnapshot snapshot,
            TimeSpan ttl)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            return cache.SetStringAsync(key, JsonSerializer.Serialize(snapshot), options);
        }

        private sealed class RateLimitSnapshot
        {
            public int Count { get; set; }
            public DateTimeOffset WindowEnd { get; set; }
        }
    }
}
