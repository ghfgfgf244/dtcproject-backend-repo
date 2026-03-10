using dtc.Application.DTOs.Collaborators;
using dtc.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Collaborators
{
    public interface ICollaboratorService
    {
        Task<IEnumerable<UserResponseDto>> GetCollaboratorListAsync();
        Task<ReferralCodeResponseDto> GetMyReferralCodeAsync(Guid collaboratorId);
        Task<ReferralCodeResponseDto> GenerateReferralCodeAsync(Guid collaboratorId, string code);
        Task<int> GetTokenUsageCountAsync(Guid collaboratorId);
        Task<decimal> GetCommissionRateAsync();
        Task<IEnumerable<CollaboratorCommissionResponseDto>> CalculateAndGetCommissionsAsync(Guid collaboratorId);
        Task<IEnumerable<CollaboratorCommissionResponseDto>> GetMyCommissionsAsync(Guid collaboratorId);
    }
}
