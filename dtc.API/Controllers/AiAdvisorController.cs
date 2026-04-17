using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;
using dtc.Application.Features.AI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dtc.API.Controllers
{
    public class AiAdvisorController : BaseApiController
    {
        private readonly ICourseAdvisorService _courseAdvisorService;
        private readonly ITheoryAssistantService _theoryAssistantService;
        private readonly IDashboardInsightService _dashboardInsightService;

        public AiAdvisorController(
            ICourseAdvisorService courseAdvisorService,
            ITheoryAssistantService theoryAssistantService,
            IDashboardInsightService dashboardInsightService)
        {
            _courseAdvisorService = courseAdvisorService;
            _theoryAssistantService = theoryAssistantService;
            _dashboardInsightService = dashboardInsightService;
        }

        [HttpPost("course")]
        public async Task<IActionResult> GetCourseAdvice(
            [FromBody] CourseAdvisorRequestDto request,
            CancellationToken cancellationToken)
        {
            var response = await _courseAdvisorService.AdviseAsync(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("theory")]
        public async Task<IActionResult> AskTheory(
            [FromBody] TheoryAssistantRequestDto request,
            CancellationToken cancellationToken)
        {
            var response = await _theoryAssistantService.AskAsync(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("dashboard-summary")]
        public async Task<IActionResult> SummarizeDashboard(
            [FromBody] DashboardInsightRequestDto request,
            CancellationToken cancellationToken)
        {
            var response = await _dashboardInsightService.SummarizeAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
