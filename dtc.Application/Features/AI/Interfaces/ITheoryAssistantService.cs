using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;

namespace dtc.Application.Features.AI.Interfaces
{
    public interface ITheoryAssistantService
    {
        Task<TheoryAssistantResponseDto> AskAsync(
            TheoryAssistantRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
