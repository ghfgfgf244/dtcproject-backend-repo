using dtc.Application.DTOs.Users;
using dtc.Application.Interfaces;
using dtc.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace dtc.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Phone = user.Phone?.Value ?? "",
                LastLogin = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }
    }
}
