using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dtc.API.Controllers
{
    [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
    public class KnowledgeController : BaseApiController
    {
        private readonly IKnowledgeChunkService _knowledgeChunkService;

        public KnowledgeController(IKnowledgeChunkService knowledgeChunkService)
        {
            _knowledgeChunkService = knowledgeChunkService;
        }

        [HttpPost("reindex/questions")]
        public async Task<IActionResult> ReindexQuestions(CancellationToken cancellationToken)
        {
            var count = await _knowledgeChunkService.ReindexQuestionsAsync(cancellationToken);
            return Ok(count, "Question knowledge reindexed.");
        }

        [HttpPost("reindex/resources")]
        public async Task<IActionResult> ReindexResources(CancellationToken cancellationToken)
        {
            var count = await _knowledgeChunkService.ReindexResourcesAsync(cancellationToken);
            return Ok(count, "Resource knowledge reindexed.");
        }

        [HttpPost("reindex/blogs")]
        public async Task<IActionResult> ReindexBlogs(CancellationToken cancellationToken)
        {
            var count = await _knowledgeChunkService.ReindexBlogsAsync(cancellationToken);
            return Ok(count, "Blog knowledge reindexed.");
        }

        [HttpPost("reindex/courses")]
        public async Task<IActionResult> ReindexCourses(CancellationToken cancellationToken)
        {
            var count = await _knowledgeChunkService.ReindexCoursesAsync(cancellationToken);
            return Ok(count, "Course knowledge reindexed.");
        }
    }
}
