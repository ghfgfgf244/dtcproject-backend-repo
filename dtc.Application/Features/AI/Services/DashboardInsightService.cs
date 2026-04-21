using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;
using dtc.Application.Features.AI.Interfaces;

namespace dtc.Application.Features.AI.Services
{
    public class DashboardInsightService : IDashboardInsightService
    {
        private readonly IAiRouterService _aiRouterService;

        public DashboardInsightService(IAiRouterService aiRouterService)
        {
            _aiRouterService = aiRouterService;
        }

        public async Task<DashboardInsightResponseDto> SummarizeAsync(
            DashboardInsightRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var result = await _aiRouterService.GenerateAsync(
                "dashboard-summary",
                $"Tom tat dashboard cho role {request.Role}. Du lieu ngu canh: {request.ContextJson ?? "{}"}",
                cancellationToken);

            return new DashboardInsightResponseDto
            {
                Summary = result.Content,
                Model = result.Model
            };
        }
    }
}
