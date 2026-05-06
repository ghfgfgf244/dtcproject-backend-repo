using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;
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
                    Text = "He thong AI chua duoc cau hinh san sang. Vui long thu lai sau.",
                    UsedMockResponse = true
                };
            }

            var configuredKeys = _geminiSettings.ApiKeys
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .ToList();

            var availableKeys = new System.Collections.Generic.List<string>();
            foreach (var apiKey in configuredKeys)
            {
                var keyHash = RedisApiKeyRotationStore.HashKey(apiKey);
                if (!await _keyRotationStore.IsCoolingDownAsync(keyHash, cancellationToken))
                {
                    availableKeys.Add(apiKey);
                }
            }

            // If every key is currently cooling down, retry them once instead of hard failing.
            // This prevents the whole AI layer from becoming unavailable due to stale or overly broad cooldowns.
            var candidateKeys = availableKeys.Count > 0 ? availableKeys : configuredKeys;
            string? lastErrorMessage = null;

            foreach (var apiKey in candidateKeys)
            {
                var keyHash = RedisApiKeyRotationStore.HashKey(apiKey);

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

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorPayload = await response.Content.ReadAsStringAsync(cancellationToken);
                        lastErrorMessage = BuildErrorMessage(model, response.StatusCode, errorPayload);

                        if (ShouldMarkCooldown(response.StatusCode))
                        {
                            await _keyRotationStore.MarkCooldownAsync(
                                keyHash,
                                _geminiSettings.CooldownMinutesWhenRateLimited,
                                cancellationToken);
                        }

                        continue;
                    }

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
                    lastErrorMessage = $"Khong the ket noi toi Gemini model {model}.";
                    await _keyRotationStore.MarkCooldownAsync(
                        keyHash,
                        _geminiSettings.CooldownMinutesWhenRateLimited,
                        cancellationToken);
                }
            }

            return new GeminiTextResult
            {
                Model = model,
                Text = string.IsNullOrWhiteSpace(lastErrorMessage)
                    ? "Tam thoi khong the ket noi den dich vu AI. Vui long thu lai sau."
                    : lastErrorMessage,
                UsedMockResponse = true
            };
        }

        private static bool ShouldMarkCooldown(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.TooManyRequests
                || statusCode == HttpStatusCode.ServiceUnavailable
                || statusCode == HttpStatusCode.BadGateway
                || statusCode == HttpStatusCode.GatewayTimeout;
        }

        private static string BuildErrorMessage(string model, HttpStatusCode statusCode, string? errorPayload)
        {
            if (statusCode == HttpStatusCode.Unauthorized || statusCode == HttpStatusCode.Forbidden)
            {
                return $"API key Gemini khong hop le hoac khong du quyen voi model {model}.";
            }

            if (statusCode == HttpStatusCode.TooManyRequests)
            {
                return $"Model {model} dang tam qua tai hoac vuot gioi han truy cap.";
            }

            if (statusCode == HttpStatusCode.NotFound)
            {
                return $"Model {model} khong ton tai hoac khong kha dung trong cau hinh hien tai.";
            }

            if (statusCode == HttpStatusCode.BadRequest)
            {
                return $"Yeu cau gui toi model {model} khong hop le.";
            }

            return string.IsNullOrWhiteSpace(errorPayload)
                ? $"Khong the nhan phan hoi hop le tu model {model}. (HTTP {(int)statusCode})"
                : $"Khong the nhan phan hoi hop le tu model {model}. (HTTP {(int)statusCode})";
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
