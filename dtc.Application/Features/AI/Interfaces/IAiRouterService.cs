using System.Threading;
using System.Threading.Tasks;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IAiRouterService
    {
        Task<AiGenerationResult> GenerateAsync(
            string useCase,
            string prompt,
            CancellationToken cancellationToken = default);
    }

    public class AiGenerationResult
    {
        public string Model { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool UsedFallback { get; set; }
    }
}
