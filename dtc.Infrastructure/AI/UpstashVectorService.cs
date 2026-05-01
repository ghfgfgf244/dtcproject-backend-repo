using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;
using dtc.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace dtc.Infrastructure.AI
{
    public class UpstashVectorService : IVectorSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly UpstashVectorSettings _settings;
        private readonly AiSettings _aiSettings;

        public UpstashVectorService(
            HttpClient httpClient,
            IOptions<UpstashVectorSettings> settings,
            IOptions<AiSettings> aiSettings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _aiSettings = aiSettings.Value;

            if (!string.IsNullOrWhiteSpace(_settings.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _settings.Token);
            }
        }

        public async Task UpsertAsync(KnowledgeVectorDocument document, CancellationToken cancellationToken = default)
        {
            if (document == null || string.IsNullOrWhiteSpace(document.Id) || document.Embedding.Length == 0)
            {
                return;
            }

            if (!HasValidConfiguration())
            {
                return;
            }

            var payload = new UpstashUpsertRequest
            {
                Id = document.Id,
                Vector = document.Embedding,
                Metadata = document.Metadata ?? [],
                Data = document.Text
            };

            using var response = await _httpClient.PostAsJsonAsync("/upsert", payload, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IReadOnlyCollection<KnowledgeVectorSearchResult>> SearchAsync(
            string query,
            float[]? embedding = null,
            IReadOnlyDictionary<string, string>? metadataFilters = null,
            int topK = 5,
            CancellationToken cancellationToken = default)
        {
            if ((embedding == null || embedding.Length == 0) || !HasValidConfiguration())
            {
                return [];
            }

            var payload = new UpstashQueryRequest
            {
                Vector = embedding,
                TopK = Math.Max(1, topK <= 0 ? _settings.DefaultTopK : topK),
                IncludeData = true,
                IncludeMetadata = true,
                Filter = BuildFilter(metadataFilters)
            };

            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/query", payload, cancellationToken);
                response.EnsureSuccessStatusCode();

                var items = await response.Content.ReadFromJsonAsync<List<UpstashQueryResponseItem>>(cancellationToken: cancellationToken)
                    ?? [];

                return items.Select(item => new KnowledgeVectorSearchResult
                    {
                        Id = item.Id ?? string.Empty,
                        Text = item.Data ?? string.Empty,
                        Score = item.Score,
                        Metadata = item.Metadata ?? []
                    })
                    .ToList();
            }
            catch (HttpRequestException)
            {
                return [];
            }
        }

        private bool HasValidConfiguration()
        {
            return !_aiSettings.EnableMockResponses
                && !string.IsNullOrWhiteSpace(_settings.Endpoint)
                && !string.IsNullOrWhiteSpace(_settings.Token);
        }

        private static string? BuildFilter(IReadOnlyDictionary<string, string>? metadataFilters)
        {
            if (metadataFilters == null || metadataFilters.Count == 0)
            {
                return null;
            }

            var filters = metadataFilters
                .Where(static pair => !string.IsNullOrWhiteSpace(pair.Key) && !string.IsNullOrWhiteSpace(pair.Value))
                .Select(pair => $"{pair.Key} = '{EscapeFilterValue(pair.Value)}'")
                .ToArray();

            return filters.Length == 0 ? null : string.Join(" AND ", filters);
        }

        private static string EscapeFilterValue(string value)
        {
            return value.Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("'", "\\'", StringComparison.Ordinal);
        }

        private sealed class UpstashUpsertRequest
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("vector")]
            public float[] Vector { get; set; } = [];

            [JsonPropertyName("metadata")]
            public Dictionary<string, string> Metadata { get; set; } = [];

            [JsonPropertyName("data")]
            public string Data { get; set; } = string.Empty;
        }

        private sealed class UpstashQueryRequest
        {
            [JsonPropertyName("vector")]
            public float[] Vector { get; set; } = [];

            [JsonPropertyName("topK")]
            public int TopK { get; set; }

            [JsonPropertyName("includeData")]
            public bool IncludeData { get; set; }

            [JsonPropertyName("includeMetadata")]
            public bool IncludeMetadata { get; set; }

            [JsonPropertyName("filter")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Filter { get; set; }
        }

        private sealed class UpstashQueryResponseItem
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("score")]
            public double Score { get; set; }

            [JsonPropertyName("metadata")]
            public Dictionary<string, string>? Metadata { get; set; }

            [JsonPropertyName("data")]
            public string? Data { get; set; }
        }
    }
}
