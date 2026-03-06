using dtc.Application.DTOs.Users;
using System;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
        Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request);
        Task DeleteMyProfileAsync(Guid userId);
        Task ToggleUserStatusAsync(Guid adminId, Guid targetUserId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    }
}
