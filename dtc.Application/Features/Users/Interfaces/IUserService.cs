using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
        Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request);
        Task DeleteMyProfileAsync(Guid userId);
        Task ToggleUserStatusAsync(Guid adminId, Guid targetUserId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(int roleId);
        Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request, Guid adminId);
        Task AddStudentRoleAsync(Guid userId);
        Task UpdateUserRolesAsync(Guid targetUserId, UpdateUserRolesRequestDto request);
        Task ApplyForStaffAsync(Guid userId, ApplyStaffRequestDto request);
    }
}
