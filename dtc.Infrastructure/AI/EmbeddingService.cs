using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;

namespace dtc.Infrastructure.AI
{
    public class EmbeddingService : IEmbeddingService
    {
        public Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult(Array.Empty<float>());
            }

            var bytes = Encoding.UTF8.GetBytes(text);
            var vector = bytes.Take(32).Select(b => b / 255f).ToArray();
            return Task.FromResult(vector);
        }
    }
}
