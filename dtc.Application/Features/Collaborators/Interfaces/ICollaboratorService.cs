using dtc.Application.Features.Collaborators.Interfaces;
using dtc.Application.Features.Collaborators.DTOs;
using dtc.Application.Features.Collaborators.Interfaces;
using dtc.Application.Features.Collaborators.DTOs;
using dtc.Application.Features.Collaborators.Interfaces;
using dtc.Application.Features.Collaborators.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Collaborators.Interfaces
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
