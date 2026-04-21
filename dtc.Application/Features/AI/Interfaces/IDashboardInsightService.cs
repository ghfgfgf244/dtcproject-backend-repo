using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface IDashboardInsightService
    {
        Task<DashboardInsightResponseDto> SummarizeAsync(
            DashboardInsightRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
