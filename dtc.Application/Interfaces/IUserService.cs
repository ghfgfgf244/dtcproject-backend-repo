using dtc.Application.DTOs.Users;
using System;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    }
}
