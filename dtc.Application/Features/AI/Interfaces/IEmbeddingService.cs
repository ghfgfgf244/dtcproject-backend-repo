using System.Threading;
using System.Threading.Tasks;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(
            string text,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = default);
    }

    public sealed class EmbeddingGenerationOptions
    {
        public string? TaskType { get; init; }
        public string? Title { get; init; }
    }
}
