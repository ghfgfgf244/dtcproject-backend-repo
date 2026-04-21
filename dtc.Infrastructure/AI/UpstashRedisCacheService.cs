using System;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace dtc.Infrastructure.AI
{
    public class UpstashRedisCacheService : IAiCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public UpstashRedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
            => _distributedCache.GetStringAsync(key, cancellationToken);

        public Task SetStringAsync(string key, string value, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            return _distributedCache.SetStringAsync(key, value, options, cancellationToken);
        }
    }
}
