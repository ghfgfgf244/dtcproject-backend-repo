using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface ICourseAdvisorService
    {
        Task<CourseAdvisorResponseDto> AdviseAsync(
            CourseAdvisorRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
