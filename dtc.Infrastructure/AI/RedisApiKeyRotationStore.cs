using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;
using dtc.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace dtc.Infrastructure.AI
{
    public class RedisApiKeyRotationStore : IApiKeyRotationStore
    {
        private readonly IAiCacheService _cacheService;
        private readonly UpstashRedisSettings _settings;

        public RedisApiKeyRotationStore(
            IAiCacheService cacheService,
            IOptions<UpstashRedisSettings> settings)
        {
            _cacheService = cacheService;
            _settings = settings.Value;
        }

        public async Task<bool> IsCoolingDownAsync(string keyHash, CancellationToken cancellationToken = default)
        {
            var value = await _cacheService.GetStringAsync(BuildKey(keyHash), cancellationToken);
            return !string.IsNullOrWhiteSpace(value);
        }

        public Task MarkCooldownAsync(string keyHash, int cooldownMinutes, CancellationToken cancellationToken = default)
        {
            return _cacheService.SetStringAsync(
                BuildKey(keyHash),
                "1",
                TimeSpan.FromMinutes(Math.Max(1, cooldownMinutes)),
                cancellationToken);
        }

        public static string HashKey(string apiKey)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
            return Convert.ToHexString(bytes);
        }

        private string BuildKey(string keyHash) => $"{_settings.KeyPrefix}:key-cooldown:{keyHash}";
    }
}
