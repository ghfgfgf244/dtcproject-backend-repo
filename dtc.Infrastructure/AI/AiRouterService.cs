using System;
using System.Linq;
using System.Text.RegularExpressions;
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
            var normalizedPrompt =
                "Chỉ được trả lời bằng tiếng Việt có dấu. " +
                "Tuyệt đối không dùng tiếng Anh nếu người dùng không yêu cầu. " +
                "Không được lặp lại prompt, role, constraints, language, context hay hướng dẫn hệ thống. " +
                "Trả lời trực tiếp, ngắn gọn, rõ ràng và dễ đọc. " +
                "Nếu cần chia mục thì chỉ giữ các tiêu đề nội dung cuối cùng bằng tiếng Việt. " +
                "Nội dung cần xử lý: " + prompt;

            var cacheKey = $"dtc:ai:response:v2:{useCase}:{_settings.DefaultModel}:{normalizedPrompt.GetHashCode()}";
            var cached = await _cacheService.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                return new AiGenerationResult
                {
                    Content = cached,
                    Model = _settings.DefaultModel
                };
            }

            var primary = await _geminiClient.GenerateTextAsync(_settings.DefaultModel, normalizedPrompt, cancellationToken);
            if (!string.IsNullOrWhiteSpace(primary.Text))
            {
                var cleanedPrimary = NormalizeAiContent(primary.Text);

                await _cacheService.SetStringAsync(
                    cacheKey,
                    cleanedPrimary,
                    TimeSpan.FromMinutes(Math.Max(1, _settings.CacheMinutes)),
                    cancellationToken);

                return new AiGenerationResult
                {
                    Content = cleanedPrimary,
                    Model = primary.Model
                };
            }

            var fallback = await _geminiClient.GenerateTextAsync(_settings.FallbackModel, normalizedPrompt, cancellationToken);
            var cleanedFallback = NormalizeAiContent(fallback.Text);
            return new AiGenerationResult
            {
                Content = cleanedFallback,
                Model = fallback.Model,
                UsedFallback = true
            };
        }

        private static string NormalizeAiContent(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            var lines = content
                .Replace("\r", string.Empty)
                .Split('\n')
                .Select(static line => line.Trim())
                .Where(static line => !string.IsNullOrWhiteSpace(line))
                .Where(static line => !ShouldSkipLine(line))
                .Select(NormalizeLine)
                .Where(static line => !string.IsNullOrWhiteSpace(line) && line != "-")
                .ToList();

            if (lines.Count == 0)
            {
                return string.Empty;
            }

            var firstHeadingIndex = lines.FindIndex(IsMeaningfulHeading);
            if (firstHeadingIndex >= 0)
            {
                lines = lines.Skip(firstHeadingIndex).ToList();
            }

            return string.Join("\n", lines);
        }

        private static bool ShouldSkipLine(string line)
        {
            var normalized = line.Trim().ToLowerInvariant();
            var blockedMarkers = new[]
            {
                "user constraints",
                "mandatory output format",
                "content of the problem",
                "note:",
                "wait,",
                "revised content",
                "check sections",
                "final polish",
                "role:",
                "language:",
                "input question:",
                "question:",
                "context:",
                "constraints",
                "only vietnamese",
                "no english unless requested",
                "required addition",
                "retrieved context",
                "nature of the question",
                "this is a very broad",
                "driving license theory assistant",
                "mandatory for every answer",
                "the user asked",
                "the context provided was",
                "i will",
                "since the persona is"
            };

            return blockedMarkers.Any(marker => normalized.Contains(marker, StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeLine(string line)
        {
            var cleaned = line
                .Replace("**", string.Empty)
                .Replace("*", string.Empty)
                .Trim();

            cleaned = Regex.Replace(
                cleaned,
                @"\s*\(max\s+\d+[^)]*\)\s*",
                string.Empty,
                RegexOptions.IgnoreCase);

            cleaned = Regex.Replace(
                cleaned,
                @"\s*\(how to drive\)\s*",
                string.Empty,
                RegexOptions.IgnoreCase);

            cleaned = Regex.Replace(
                cleaned,
                @"\s*-\s*ok\s*$",
                string.Empty,
                RegexOptions.IgnoreCase);

            cleaned = cleaned.Trim(':', '-', ' ');
            var lower = cleaned.ToLowerInvariant();

            return lower switch
            {
                "giai thich" => "Giải thích:",
                "can ghi nho" => "Cần ghi nhớ:",
                "meo hoc nhanh" => "Mẹo học nhanh:",
                "tong quan" => "Tổng quan:",
                "top goi y" => "Top gợi ý:",
                "loi khuyen tiep theo" => "Lời khuyên tiếp theo:",
                "van de" => "Vấn đề:",
                "goi y xu ly" => "Gợi ý xử lý:",
                _ => line.StartsWith("-") ? $"- {cleaned}" : cleaned
            };
        }

        private static bool IsMeaningfulHeading(string line)
        {
            var normalized = line.Trim().ToLowerInvariant();
            return normalized is
                "giải thích:" or "giai thich:" or
                "cần ghi nhớ:" or "can ghi nho:" or
                "mẹo học nhanh:" or "meo hoc nhanh:" or
                "tổng quan:" or "tong quan:" or
                "top gợi ý:" or "top goi y:" or
                "lời khuyên tiếp theo:" or "loi khuyen tiep theo:" or
                "vấn đề:" or "van de:" or
                "gợi ý xử lý:" or "goi y xu ly:" ||
                Regex.IsMatch(line, @"^(I|II|III|IV|V|VI|VII|VIII|IX|X)\.\s+");
        }
    }
}
