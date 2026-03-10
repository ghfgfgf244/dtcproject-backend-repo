using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.DTOs.Training.Terms;

namespace dtc.Application.Interfaces.Training
{
    public interface ITermService
    {
        Task<TermResponseDto> CreateTermAsync(CreateTermRequestDto request, Guid adminId);
        Task<TermResponseDto> UpdateTermAsync(Guid termId, UpdateTermRequestDto request, Guid adminId);
        
        Task<IEnumerable<TermResponseDto>> GetAllTermsAsync();
        Task<TermResponseDto> GetTermDetailAsync(Guid termId);
        Task<bool> DeleteTermAsync(Guid termId, Guid adminId);
    }
}
