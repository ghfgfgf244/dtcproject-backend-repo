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
    public class GeminiClient : IGeminiClient
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _geminiSettings;
        private readonly AiSettings _aiSettings;
        private readonly IApiKeyRotationStore _keyRotationStore;

        public GeminiClient(
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

        public async Task<GeminiTextResult> GenerateTextAsync(
            string model,
            string prompt,
            CancellationToken cancellationToken = default)
        {
            if (_aiSettings.EnableMockResponses || _geminiSettings.ApiKeys.Count == 0)
            {
                return new GeminiTextResult
                {
                    Model = model,
                    Text = $"[AI scaffold] Chua cau hinh Gemini key. Prompt nhan duoc: {prompt}",
                    UsedMockResponse = true
                };
            }

            foreach (var apiKey in _geminiSettings.ApiKeys.Where(k => !string.IsNullOrWhiteSpace(k)))
            {
                var keyHash = RedisApiKeyRotationStore.HashKey(apiKey);
                if (await _keyRotationStore.IsCoolingDownAsync(keyHash, cancellationToken))
                {
                    continue;
                }

                try
                {
                    var endpoint = $"/v1beta/models/{model}:generateContent?key={apiKey}";
                    var request = new GeminiGenerateContentRequest
                    {
                        Contents =
                        [
                            new GeminiContent
                            {
                                Parts = [new GeminiPart { Text = prompt }]
                            }
                        ]
                    };

                    var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    var payload = await response.Content.ReadFromJsonAsync<GeminiGenerateContentResponse>(cancellationToken: cancellationToken);
                    var text = payload?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                        ?? "Khong nhan duoc noi dung tra loi tu Gemini.";

                    return new GeminiTextResult
                    {
                        Model = model,
                        Text = text
                    };
                }
                catch (HttpRequestException)
                {
                    await _keyRotationStore.MarkCooldownAsync(
                        keyHash,
                        _geminiSettings.CooldownMinutesWhenRateLimited,
                        cancellationToken);
                }
            }

            return new GeminiTextResult
            {
                Model = model,
                Text = "[AI scaffold] Tat ca API key tam thoi khong kha dung.",
                UsedMockResponse = true
            };
        }

        private sealed class GeminiGenerateContentRequest
        {
            [JsonPropertyName("contents")]
            public GeminiContent[] Contents { get; set; } = [];
        }

        private sealed class GeminiGenerateContentResponse
        {
            [JsonPropertyName("candidates")]
            public GeminiCandidate[]? Candidates { get; set; }
        }

        private sealed class GeminiCandidate
        {
            [JsonPropertyName("content")]
            public GeminiContent? Content { get; set; }
        }

        private sealed class GeminiContent
        {
            [JsonPropertyName("parts")]
            public GeminiPart[] Parts { get; set; } = [];
        }

        private sealed class GeminiPart
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
        }
    }
}
