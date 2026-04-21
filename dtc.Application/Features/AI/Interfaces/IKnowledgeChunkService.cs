using System.Threading;
using System.Threading.Tasks;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IKnowledgeChunkService
    {
        Task<int> ReindexQuestionsAsync(CancellationToken cancellationToken = default);
        Task<int> ReindexResourcesAsync(CancellationToken cancellationToken = default);
        Task<int> ReindexBlogsAsync(CancellationToken cancellationToken = default);
        Task<int> ReindexCoursesAsync(CancellationToken cancellationToken = default);
    }
}
