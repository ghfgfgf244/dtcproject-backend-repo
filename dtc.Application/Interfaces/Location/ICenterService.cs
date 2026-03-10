using dtc.Application.DTOs.Location.Centers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Location
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
