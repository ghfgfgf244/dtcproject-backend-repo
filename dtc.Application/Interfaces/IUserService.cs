using dtc.Application.DTOs.Users;
using System;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
        Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    }
}
