using System.Threading;
using System.Threading.Tasks;

namespace dtc.Infrastructure.AI
{
    public interface IApiKeyRotationStore
    {
        Task<bool> IsCoolingDownAsync(string keyHash, CancellationToken cancellationToken = default);
        Task MarkCooldownAsync(string keyHash, int cooldownMinutes, CancellationToken cancellationToken = default);
    }
}
