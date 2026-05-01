using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;
using dtc.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace dtc.Infrastructure.AI
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _geminiSettings;
        private readonly AiSettings _aiSettings;
        private readonly IApiKeyRotationStore _keyRotationStore;

        public EmbeddingService(
            HttpClient httpClient,
            IOptions<GeminiSettings> geminiSettings,
            IOptions<AiSettings> aiSettings,
            IApiKeyRotationStore keyRotationStore)
        {
            _httpClient = httpClient;
            _geminiSettings = geminiSettings.Value;
            _aiSettings = aiSettings.Value;
            _keyRotationStore = keyRotationStore;
        }

        public async Task<float[]> GenerateEmbeddingAsync(
            string text,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return [];
            }

            if (_aiSettings.EnableMockResponses)
            {
                return BuildFallbackEmbedding(text);
            }

            var model = string.IsNullOrWhiteSpace(_geminiSettings.EmbeddingModel)
                ? "gemini-embedding-001"
                : _geminiSettings.EmbeddingModel.Trim();

            foreach (var apiKey in _geminiSettings.ApiKeys.Where(static key => !string.IsNullOrWhiteSpace(key)))
            {
                var keyHash = RedisApiKeyRotationStore.HashKey(apiKey);
                if (await _keyRotationStore.IsCoolingDownAsync(keyHash, cancellationToken))
                {
                    continue;
                }

                try
                {
                    var endpoint = $"/v1beta/models/{model}:embedContent?key={apiKey}";
                    var request = new GeminiEmbedContentRequest
                    {
                        Model = $"models/{model}",
                        TaskType = string.IsNullOrWhiteSpace(options?.TaskType) ? null : options.TaskType,
                        Title = string.IsNullOrWhiteSpace(options?.Title) ? null : options.Title,
                        OutputDimensionality = _geminiSettings.EmbeddingDimensions > 0
                            ? _geminiSettings.EmbeddingDimensions
                            : null,
                        Content = new GeminiEmbedContent
                        {
                            Parts =
                            [
                                new GeminiEmbedPart
                                {
                                    Text = text
                                }
                            ]
                        }
                    };

                    var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    var payload = await response.Content.ReadFromJsonAsync<GeminiEmbedContentResponse>(cancellationToken: cancellationToken);
                    var values = payload?.Embedding?.Values;
                    if (values is { Length: > 0 })
                    {
                        return values;
                    }
                }
                catch (HttpRequestException)
                {
                    await _keyRotationStore.MarkCooldownAsync(
                        keyHash,
                        _geminiSettings.CooldownMinutesWhenRateLimited,
                        cancellationToken);
                }
            }

            return BuildFallbackEmbedding(text);
        }

        private float[] BuildFallbackEmbedding(string text)
        {
            var values = new float[Math.Max(32, _geminiSettings.EmbeddingDimensions > 0 ? _geminiSettings.EmbeddingDimensions : 32)];
            foreach (var (character, index) in text.Take(512).Select((character, index) => (character, index)))
            {
                values[index % values.Length] += character / 255f;
            }

            return values;
        }

        private sealed class GeminiEmbedContentRequest
        {
            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("content")]
            public GeminiEmbedContent Content { get; set; } = new();

            [JsonPropertyName("taskType")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? TaskType { get; set; }

            [JsonPropertyName("title")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Title { get; set; }

            [JsonPropertyName("outputDimensionality")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? OutputDimensionality { get; set; }
        }

        private sealed class GeminiEmbedContent
        {
            [JsonPropertyName("parts")]
            public GeminiEmbedPart[] Parts { get; set; } = [];
        }

        private sealed class GeminiEmbedPart
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
        }

        private sealed class GeminiEmbedContentResponse
        {
            [JsonPropertyName("embedding")]
            public GeminiEmbeddingPayload? Embedding { get; set; }
        }

        private sealed class GeminiEmbeddingPayload
        {
            [JsonPropertyName("values")]
            public float[]? Values { get; set; }
        }
    }
}
