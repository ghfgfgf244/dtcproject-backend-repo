using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IVectorSearchService
    {
        Task UpsertAsync(KnowledgeVectorDocument document, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<KnowledgeVectorSearchResult>> SearchAsync(
            string query,
            float[]? embedding = null,
            IReadOnlyDictionary<string, string>? metadataFilters = null,
            int topK = 5,
            CancellationToken cancellationToken = default);
    }

    public class KnowledgeVectorDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = [];
        public Dictionary<string, string> Metadata { get; set; } = [];
    }

    public class KnowledgeVectorSearchResult
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public double Score { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = [];
    }
}
