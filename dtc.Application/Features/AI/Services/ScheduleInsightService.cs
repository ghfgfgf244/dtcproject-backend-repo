using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;
using dtc.Application.Features.AI.Interfaces;

namespace dtc.Application.Features.AI.Services
{
    public class ScheduleInsightService : IScheduleInsightService
    {
        private readonly IAiRouterService _aiRouterService;

        public ScheduleInsightService(IAiRouterService aiRouterService)
        {
            _aiRouterService = aiRouterService;
        }

        public async Task<ScheduleInsightResponseDto> ExplainAsync(
            ScheduleInsightRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var result = await _aiRouterService.GenerateAsync(
                "schedule-insight",
                $"Phan tich xung dot lich hoc sau va dua ra goi y: {request.Scenario}. {request.Context}",
                cancellationToken);

            return new ScheduleInsightResponseDto
            {
                Summary = result.Content,
                Model = result.Model
            };
        }
    }
}
