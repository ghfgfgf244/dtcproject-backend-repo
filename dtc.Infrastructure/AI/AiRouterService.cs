using System;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;
using dtc.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace dtc.Infrastructure.AI
{
    public class AiRouterService : IAiRouterService
    {
        private readonly IGeminiClient _geminiClient;
        private readonly IAiCacheService _cacheService;
        private readonly AiSettings _settings;

        public AiRouterService(
            IGeminiClient geminiClient,
            IAiCacheService cacheService,
            IOptions<AiSettings> settings)
        {
            _geminiClient = geminiClient;
            _cacheService = cacheService;
            _settings = settings.Value;
        }

        public async Task<AiGenerationResult> GenerateAsync(
            string useCase,
            string prompt,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"dtc:ai:response:{useCase}:{prompt.GetHashCode()}";
            var cached = await _cacheService.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                return new AiGenerationResult
                {
                    Content = cached,
                    Model = _settings.DefaultModel
                };
            }

            var primary = await _geminiClient.GenerateTextAsync(_settings.DefaultModel, prompt, cancellationToken);
            if (!string.IsNullOrWhiteSpace(primary.Text))
            {
                await _cacheService.SetStringAsync(
                    cacheKey,
                    primary.Text,
                    TimeSpan.FromMinutes(Math.Max(1, _settings.CacheMinutes)),
                    cancellationToken);

                return new AiGenerationResult
                {
                    Content = primary.Text,
                    Model = primary.Model
                };
            }

            var fallback = await _geminiClient.GenerateTextAsync(_settings.FallbackModel, prompt, cancellationToken);
            return new AiGenerationResult
            {
                Content = fallback.Text,
                Model = fallback.Model,
                UsedFallback = true
            };
        }
    }
}
