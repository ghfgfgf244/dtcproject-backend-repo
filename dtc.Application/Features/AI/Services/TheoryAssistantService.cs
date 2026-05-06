using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;
using dtc.Application.Features.AI.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace dtc.Application.Features.AI.Services
{
    public class TheoryAssistantService : ITheoryAssistantService
    {
        private readonly IAiRouterService _aiRouterService;
        private readonly IVectorSearchService _vectorSearchService;
        private readonly IEmbeddingService _embeddingService;

        public TheoryAssistantService(
            IAiRouterService aiRouterService,
            IVectorSearchService vectorSearchService,
            IEmbeddingService embeddingService)
        {
            _aiRouterService = aiRouterService;
            _vectorSearchService = vectorSearchService;
            _embeddingService = embeddingService;
        }

        public async Task<TheoryAssistantResponseDto> AskAsync(
            TheoryAssistantRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return CreateFallbackResponse(
                    "Vui lòng nhập câu hỏi lý thuyết để trợ lý AI có thể hỗ trợ.");
            }

            var filters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                filters["category"] = request.Category.Trim();
            }

            IReadOnlyCollection<KnowledgeVectorSearchResult> retrievalResults = [];

            try
            {
                var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(
                    request.Question,
                    new EmbeddingGenerationOptions
                    {
                        TaskType = "RETRIEVAL_QUERY"
                    },
                    cancellationToken);

                retrievalResults = await _vectorSearchService.SearchAsync(
                    request.Question,
                    queryEmbedding,
                    filters,
                    topK: 4,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                retrievalResults = [];
            }

            var context = string.Join(
                "\n\n",
                retrievalResults.Select((item, index) => $"Nguon {index + 1}:\n{item.Text}"));

            var prompt =
                $"Ban la tro ly on ly thuyet GPLX. " +
                $"Hay tra loi bang tieng Viet, de hieu, dung trong tam va khong nhac lai huong dan. " +
                $"Neu cau hoi khong phai dang trac nghiem cu the, hay giai thich theo cach thuc hanh de nguoi hoc de hinh dung. " +
                $"Chi duoc dua tren du lieu truy xuat khi co; neu khong co ngu canh phu hop thi noi ro rang do. " +
                $"Nhom cau hoi: {request.Category ?? "chua ro"}. " +
                $"Hang GPLX: {request.ExamLevel ?? "chua ro"}. " +
                $"Cau hoi cua hoc vien: {request.Question}\n\n" +
                $"Ngu canh truy xuat:\n{(string.IsNullOrWhiteSpace(context) ? "Khong co ngu canh truy xuat phu hop." : context)}\n\n" +
                "Dinh dang bat buoc:\n" +
                "Tra loi ngan gon theo dung 3 muc sau:\n" +
                "Giai thich:\n" +
                "- toi da 3 y\n" +
                "Can ghi nho:\n" +
                "- toi da 3 y\n" +
                "Meo hoc nhanh:\n" +
                "- toi da 2 y\n" +
                $"{(request.IncludeStudyTips ? "Bat buoc co muc 'Meo hoc nhanh:'." : "Neu khong can, van giu cau tra loi that gon.")}";

            try
            {
                var result = await _aiRouterService.GenerateAsync("theory-assistant", prompt, cancellationToken);

                return new TheoryAssistantResponseDto
                {
                    Answer = string.IsNullOrWhiteSpace(result.Content)
                        ? "Trợ lý AI đã nhận câu hỏi nhưng chưa thể tạo câu trả lời phù hợp. Vui lòng thử lại sau."
                        : NormalizeTheoryAnswer(result.Content),
                    Model = result.Model,
                    Sources = retrievalResults
                        .Select(item => new AiSourceDto
                        {
                            Title = item.Metadata.GetValueOrDefault("title") ?? item.Id,
                            Snippet = item.Text.Length > 220 ? $"{item.Text[..220]}..." : item.Text,
                            SourceType = item.Metadata.GetValueOrDefault("documentType") ?? "knowledge",
                            ReferenceId = item.Metadata.GetValueOrDefault("referenceId")
                        })
                        .ToList(),
                    SuggestedTopics = retrievalResults
                        .Select(item => item.Metadata.GetValueOrDefault("category") ?? item.Metadata.GetValueOrDefault("courseName"))
                        .Where(item => !string.IsNullOrWhiteSpace(item))
                        .Distinct()
                        .Take(4)
                        .ToList()!
                };
            }
            catch
            {
                return CreateFallbackResponse(
                    "Hiện chưa thể lấy phản hồi từ trợ lý AI. Vui lòng thử lại sau.",
                    retrievalResults);
            }
        }

        private static string NormalizeTheoryAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                return "Hiện chưa có nội dung phù hợp để trả lời câu hỏi này.";
            }

            var rawLines = answer
                .Replace("\r", string.Empty)
                .Split('\n')
                .Select(static line => line.Trim())
                .Where(static line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var bannedMarkers = new[]
            {
                "user constraints",
                "mandatory output format",
                "content of the problem",
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
                "note:",
                "wait,",
                "revised content",
                "check sections",
                "final polish",
                "i will",
                "the user asked",
                "the context provided was",
                "since the persona is",
                " - ok",
                "— ok"
            };

            var cleanedLines = rawLines
                .Where(line => !bannedMarkers.Any(marker =>
                    line.Contains(marker, System.StringComparison.OrdinalIgnoreCase)))
                .Select(NormalizeTheoryLine)
                .Where(static line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var firstContentIndex = cleanedLines.FindIndex(static line =>
                IsMeaningfulHeading(line));

            if (firstContentIndex >= 0)
            {
                cleanedLines = cleanedLines.Skip(firstContentIndex).ToList();
            }

            if (cleanedLines.Count == 0)
            {
                return "Hiện chưa có nội dung phù hợp để trả lời câu hỏi này.";
            }

            return string.Join("\n", cleanedLines);
        }

        private static string NormalizeTheoryLine(string line)
        {
            var cleaned = line
                .Replace("**", string.Empty)
                .Replace("*", string.Empty)
                .Trim();

            cleaned = System.Text.RegularExpressions.Regex.Replace(
                cleaned,
                @"\s*\(max\s+\d+\s+bullet\s+points?\)\s*",
                string.Empty,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            cleaned = System.Text.RegularExpressions.Regex.Replace(
                cleaned,
                @"\s*\(how to drive\)\s*",
                string.Empty,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            cleaned = System.Text.RegularExpressions.Regex.Replace(
                cleaned,
                @"\s*-\s*ok\s*$",
                string.Empty,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            cleaned = cleaned.Trim(':', '-', ' ');

            var lower = cleaned.ToLowerInvariant();

            return lower switch
            {
                "giai thich" => "Giải thích:",
                "can ghi nho" => "Cần ghi nhớ:",
                "meo hoc nhanh" => "Mẹo học nhanh:",
                _ => line.StartsWith("-") ? $"- {cleaned}" : cleaned
            };
        }

        private static bool IsMeaningfulHeading(string line)
        {
            var normalized = line.Trim().ToLowerInvariant();
            return normalized is
                "giải thích:" or
                "giai thich:" or
                "giải thích" or
                "giai thich" or
                "cần ghi nhớ:" or
                "can ghi nho:" or
                "cần ghi nhớ" or
                "can ghi nho" or
                "mẹo học nhanh:" or
                "meo hoc nhanh:" or
                "mẹo học nhanh" or
                "meo hoc nhanh";
        }

        private static TheoryAssistantResponseDto CreateFallbackResponse(
            string answer,
            IReadOnlyCollection<KnowledgeVectorSearchResult>? retrievalResults = null)
        {
            var safeResults = retrievalResults ?? [];

            return new TheoryAssistantResponseDto
            {
                Answer = answer,
                Model = "fallback",
                Sources = safeResults
                    .Select(item => new AiSourceDto
                    {
                        Title = item.Metadata.GetValueOrDefault("title") ?? item.Id,
                        Snippet = item.Text.Length > 220 ? $"{item.Text[..220]}..." : item.Text,
                        SourceType = item.Metadata.GetValueOrDefault("documentType") ?? "knowledge",
                        ReferenceId = item.Metadata.GetValueOrDefault("referenceId")
                    })
                    .ToList(),
                SuggestedTopics = safeResults
                    .Select(item => item.Metadata.GetValueOrDefault("category") ?? item.Metadata.GetValueOrDefault("courseName"))
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct()
                    .Take(4)
                    .ToList()!
            };
        }
    }
}
