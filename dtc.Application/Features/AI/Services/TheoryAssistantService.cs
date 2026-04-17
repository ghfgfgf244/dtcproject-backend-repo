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
            var filters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                filters["category"] = request.Category.Trim();
            }

            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(request.Question, cancellationToken);
            var retrievalResults = await _vectorSearchService.SearchAsync(
                request.Question,
                queryEmbedding,
                filters,
                topK: 4,
                cancellationToken: cancellationToken);

            var context = string.Join(
                "\n\n",
                retrievalResults.Select((item, index) => $"Nguon {index + 1}:\n{item.Text}"));

            var prompt =
                $"Ban la tro ly huong dan on thi GPLX bang tieng Viet. " +
                $"Hay giai thich de hoc vien de hieu, noi ro dap an dung neu xac dinh duoc tu ngu canh, " +
                $"va chi dua tren du lieu truy xuat khi co. " +
                $"Nhom: {request.Category ?? "chua ro"}. " +
                $"Hang GPLX: {request.ExamLevel ?? "chua ro"}. " +
                $"Noi dung cau hoi: {request.Question}\n\n" +
                $"Ngu canh truy xuat:\n{(string.IsNullOrWhiteSpace(context) ? "Khong co ngu canh truy xuat phu hop." : context)}\n\n" +
                $"{(request.IncludeStudyTips ? "Cuoi cau tra loi them meo hoc nhanh gon." : string.Empty)}";

            var result = await _aiRouterService.GenerateAsync("theory-assistant", prompt, cancellationToken);

            return new TheoryAssistantResponseDto
            {
                Answer = result.Content,
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
    }
}
