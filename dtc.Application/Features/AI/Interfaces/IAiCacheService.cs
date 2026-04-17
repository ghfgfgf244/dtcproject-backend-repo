using System;
using System.Threading;
using System.Threading.Tasks;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IAiCacheService
    {
        Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
        Task SetStringAsync(string key, string value, TimeSpan ttl, CancellationToken cancellationToken = default);
    }
}
