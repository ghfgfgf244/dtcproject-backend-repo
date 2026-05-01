using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.ChunkBuilders;
using dtc.Application.Features.AI.Interfaces;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.AI.Services
{
    public class KnowledgeChunkService : IKnowledgeChunkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVectorSearchService _vectorSearchService;
        private readonly IEmbeddingService _embeddingService;

        public KnowledgeChunkService(
            IUnitOfWork unitOfWork,
            IVectorSearchService vectorSearchService,
            IEmbeddingService embeddingService)
        {
            _unitOfWork = unitOfWork;
            _vectorSearchService = vectorSearchService;
            _embeddingService = embeddingService;
        }

        public async Task<int> ReindexQuestionsAsync(CancellationToken cancellationToken = default)
        {
            var questions = (await _unitOfWork.Questions.GetAllAsync()).ToList();
            var payloads = questions.Select(QuestionChunkBuilder.Build).ToList();
            return await UpsertChunksAsync(payloads, cancellationToken);
        }

        public async Task<int> ReindexResourcesAsync(CancellationToken cancellationToken = default)
        {
            var resources = (await _unitOfWork.ResourceLearnings.GetAllAsync())
                .Where(item => item.IsActive)
                .ToList();
            var courseMap = (await _unitOfWork.Courses.GetAllAsync())
                .ToDictionary(item => item.Id, item => item.CourseName);

            var payloads = resources
                .Select(item => ResourceChunkBuilder.Build(item, courseMap.GetValueOrDefault(item.CourseId)))
                .ToList();

            return await UpsertChunksAsync(payloads, cancellationToken);
        }

        public async Task<int> ReindexBlogsAsync(CancellationToken cancellationToken = default)
        {
            var blogs = (await _unitOfWork.Blogs.FindAsync(item => !item.IsDeleted && item.Status)).ToList();
            var categoryMap = (await _unitOfWork.Categories.GetAllAsync())
                .ToDictionary(item => item.CategoryId, item => item.CategoryName);

            var payloads = blogs
                .Select(item => BlogChunkBuilder.Build(item, categoryMap.GetValueOrDefault(item.CategoryId)))
                .ToList();

            return await UpsertChunksAsync(payloads, cancellationToken);
        }

        public async Task<int> ReindexCoursesAsync(CancellationToken cancellationToken = default)
        {
            var courses = (await _unitOfWork.Courses.FindAsync(item => item.IsActive && !item.IsDeleted, item => item.Center)).ToList();
            var payloads = courses.Select(CourseChunkBuilder.Build).ToList();
            return await UpsertChunksAsync(payloads, cancellationToken);
        }

        private async Task<int> UpsertChunksAsync(
            IReadOnlyCollection<KnowledgeChunkPayload> payloads,
            CancellationToken cancellationToken)
        {
            foreach (var payload in payloads)
            {
                var embedding = await _embeddingService.GenerateEmbeddingAsync(
                    payload.Text,
                    new EmbeddingGenerationOptions
                    {
                        TaskType = "RETRIEVAL_DOCUMENT",
                        Title = payload.Metadata.GetValueOrDefault("title")
                    },
                    cancellationToken);
                await _vectorSearchService.UpsertAsync(new KnowledgeVectorDocument
                {
                    Id = payload.Id,
                    Text = payload.Text,
                    Embedding = embedding,
                    Metadata = payload.Metadata
                }, cancellationToken);
            }

            return payloads.Count;
        }
    }
}
