using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Domain.Entities;
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
        Task<UserResponseDto> CreateManagedUserAsync(CreateUserRequestDto request, Guid requesterId, UserRole requesterRole);
        Task<UserResponseDto> UpdateManagedUserAsync(Guid targetUserId, UpdateManagedUserRequestDto request, Guid requesterId, UserRole requesterRole);
        Task ToggleManagedUserStatusAsync(Guid targetUserId, Guid requesterId, UserRole requesterRole);
        Task DeleteManagedUserAsync(Guid targetUserId, Guid requesterId, UserRole requesterRole);
        Task AddStudentRoleAsync(Guid userId);
        Task ApplyForStaffAsync(Guid userId, ApplyStaffRequestDto request);
        Task DeleteUserAsync(Guid targetUserId);
        Task UpdateUserRolesAsync(Guid targetUserId, UpdateUserRolesRequestDto request);
        Task<UserStatsDto> GetUserStatsAsync();
        Task AssignCenterAsync(Guid userId, Guid centerId, Guid adminId);
    }
}
