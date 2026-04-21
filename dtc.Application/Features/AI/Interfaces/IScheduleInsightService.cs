using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IScheduleInsightService
    {
        Task<ScheduleInsightResponseDto> ExplainAsync(
            ScheduleInsightRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
