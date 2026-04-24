using System.Threading;
using System.Threading.Tasks;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IGeminiClient
    {
        Task<GeminiTextResult> GenerateTextAsync(
            string model,
            string prompt,
            CancellationToken cancellationToken = default);
    }

    public class GeminiTextResult
    {
        public string Model { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool UsedMockResponse { get; set; }
    }
}
