using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;

namespace dtc.Application.Features.Training.Interfaces
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
