using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;

namespace dtc.Infrastructure.AI
{
    public class UpstashVectorService : IVectorSearchService
    {
        private static readonly ConcurrentDictionary<string, KnowledgeVectorDocument> Documents = new();

        public Task UpsertAsync(KnowledgeVectorDocument document, CancellationToken cancellationToken = default)
        {
            Documents[document.Id] = new KnowledgeVectorDocument
            {
                Id = document.Id,
                Text = document.Text,
                Embedding = document.Embedding ?? [],
                Metadata = document.Metadata ?? []
            };

            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<KnowledgeVectorSearchResult>> SearchAsync(
            string query,
            float[]? embedding = null,
            IReadOnlyDictionary<string, string>? metadataFilters = null,
            int topK = 5,
            CancellationToken cancellationToken = default)
        {
            var queryEmbedding = embedding is { Length: > 0 } ? embedding : BuildFallbackEmbedding(query);

            var results = Documents.Values
                .Where(document => MatchesFilters(document.Metadata, metadataFilters))
                .Select(document => new KnowledgeVectorSearchResult
                {
                    Id = document.Id,
                    Text = document.Text,
                    Metadata = document.Metadata,
                    Score = ComputeCosineSimilarity(queryEmbedding, document.Embedding)
                })
                .OrderByDescending(item => item.Score)
                .Take(Math.Max(1, topK))
                .ToList();

            return Task.FromResult<IReadOnlyCollection<KnowledgeVectorSearchResult>>(results);
        }

        private static bool MatchesFilters(
            IReadOnlyDictionary<string, string> metadata,
            IReadOnlyDictionary<string, string>? filters)
        {
            if (filters == null || filters.Count == 0)
            {
                return true;
            }

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Value))
                {
                    continue;
                }

                if (!metadata.TryGetValue(filter.Key, out var value) ||
                    !string.Equals(value, filter.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static float[] BuildFallbackEmbedding(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return [];
            }

            var values = new float[32];
            foreach (var (character, index) in text.Take(128).Select((character, index) => (character, index)))
            {
                values[index % values.Length] += character / 255f;
            }

            return values;
        }

        private static double ComputeCosineSimilarity(float[] left, float[] right)
        {
            if (left.Length == 0 || right.Length == 0)
            {
                return 0;
            }

            var length = Math.Min(left.Length, right.Length);
            double dot = 0;
            double leftMagnitude = 0;
            double rightMagnitude = 0;

            for (var i = 0; i < length; i++)
            {
                dot += left[i] * right[i];
                leftMagnitude += left[i] * left[i];
                rightMagnitude += right[i] * right[i];
            }

            if (leftMagnitude <= 0 || rightMagnitude <= 0)
            {
                return 0;
            }

            return dot / (Math.Sqrt(leftMagnitude) * Math.Sqrt(rightMagnitude));
        }
    }
}
