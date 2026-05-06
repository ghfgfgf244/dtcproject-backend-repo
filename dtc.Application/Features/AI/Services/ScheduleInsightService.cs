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
                "Phan tich xung dot lich hoc va dua ra goi y dieu chinh bang tieng Viet. " +
                "Chi tra loi ngan gon, de doc va khong lap lai prompt. " +
                $"Tinh huong: {request.Scenario}. " +
                $"Ngu canh: {request.Context}. " +
                "Dinh dang bat buoc:\n" +
                "Van de:\n- toi da 3 y\n" +
                "Goi y xu ly:\n- toi da 3 y",
                cancellationToken);

            return new ScheduleInsightResponseDto
            {
                Summary = result.Content,
                Model = result.Model
            };
        }
    }
}
