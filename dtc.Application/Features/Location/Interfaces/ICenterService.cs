using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using dtc.Application.Features.Location.Interfaces;
using dtc.Application.Features.Location.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Location.Interfaces
{
    public interface ICenterService
    {
        Task<CenterResponseDto> CreateCenterAsync(CreateCenterRequestDto request, Guid adminId);
        Task<CenterResponseDto> UpdateCenterAsync(Guid id, UpdateCenterRequestDto request, Guid adminId);
        Task<bool> DeactivateCenterAsync(Guid id, Guid adminId);
        Task<IEnumerable<CenterResponseDto>> GetAllCentersAsync();
        Task<CenterResponseDto> GetCenterDetailAsync(Guid id);
        Task<bool> AssignUsersToCenterAsync(Guid id, AssignUsersRequestDto request, Guid adminId);
    }
}
