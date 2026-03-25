using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public UserService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
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

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            dtc.Domain.ValueObjects.PhoneNumber? phoneObj = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                phoneObj = dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone);
            }

            bool isUpdated = user.UpdateProfile(
                fullName: request.FullName,
                phone: phoneObj,
                avatarUrl: null,
                updatedBy: userId
            );

            if (isUpdated)
            {
                await _unitOfWork.SaveChangesAsync();
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

        public async Task DeleteMyProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await _unitOfWork.Users.RemoveAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleUserStatusAsync(Guid adminId, Guid targetUserId)
        {
            if (adminId == targetUserId)
            {
                throw new Exception("You cannot ban or unban yourself.");
            }

            var targetUser = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            if (targetUser == null)
            {
                throw new Exception("Target user not found.");
            }

            if (targetUser.IsActive)
            {
                targetUser.Deactivate(adminId);
            }
            else
            {
                targetUser.Activate(adminId);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => true);
            
            var result = new List<UserResponseDto>();
            foreach (var user in users)
            {
                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    FullName = user.FullName,
                    Phone = user.Phone?.Value ?? "",
                    AvatarUrl = user.AvatarUrl,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt,
                    Roles = new List<string> { user.RoleId.ToString() }
                });
            }

            return result;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(int roleId)
        {
            var users = await _unitOfWork.Users.FindAsync(u => (int)u.RoleId == roleId);
            
            var result = new List<UserResponseDto>();
            foreach (var user in users)
            {
                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    FullName = user.FullName,
                    Phone = user.Phone?.Value ?? "",
                    AvatarUrl = user.AvatarUrl,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt,
                    Roles = new List<string> { user.RoleId.ToString() }
                });
            }

            return result;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request, Guid adminId)
        {
            var targetEmail = dtc.Domain.ValueObjects.Email.Create(request.Email);
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail);
            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            var newUser = new dtc.Domain.Entities.Permissions.User(
                clerkId: request.ClerkId, // Admin should provide the ClerkId from Clerk Dashboard or inviting flow
                email: targetEmail,
                fullName: request.FullName,
                phone: dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone),
                createdBy: adminId
            );

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                var firstRoleId = request.RoleIds.First();
                if (Enum.IsDefined(typeof(dtc.Domain.Entities.UserRole), firstRoleId))
                {
                    newUser.UpdateRole((dtc.Domain.Entities.UserRole)firstRoleId);
                }
            }

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            // Side-effect: Welcome notification
            try
            {
                await _notificationService.CreateForUserAsync(
                    newUser.Id,
                    "Chào mừng bạn đến với DTC!",
                    $"Tài khoản của bạn đã được quản trị viên khởi tạo thành công.",
                    NotificationType.Welcome);
            }
            catch { }

            return new UserResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email.Value,
                FullName = newUser.FullName,
                Phone = newUser.Phone?.Value ?? "",
                AvatarUrl = newUser.AvatarUrl,
                IsActive = newUser.IsActive,
                LastLoginAt = newUser.LastLoginAt,
                Roles = new List<string> { newUser.RoleId.ToString() }
            };
        }

        public async Task AddStudentRoleAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId);
            var targetUser = user.FirstOrDefault();

            if (targetUser == null)
            {
                throw new Exception("User not found.");
            }

            if (targetUser.RoleId == dtc.Domain.Entities.UserRole.Student)
            {
                return;
            }

            targetUser.UpdateRole(dtc.Domain.Entities.UserRole.Student);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserRolesAsync(Guid targetUserId, UpdateUserRolesRequestDto request)
        {
            var userQuery = await _unitOfWork.Users.FindAsync(u => u.Id == targetUserId);
            var targetUser = userQuery.FirstOrDefault();

            if (targetUser == null)
            {
                throw new Exception("User not found.");
            }

            var requestedRoleIds = request.RoleIds.Distinct().ToList();
            if (requestedRoleIds.Contains((int)dtc.Domain.Entities.UserRole.Student))
            {
                throw new Exception("Cannot assign Student role through this endpoint. Use specific student registration flow.");
            }

            if (requestedRoleIds.Any())
            {
                var newRoleId = requestedRoleIds.First();
                if (Enum.IsDefined(typeof(dtc.Domain.Entities.UserRole), newRoleId))
                {
                    var newRole = (dtc.Domain.Entities.UserRole)newRoleId;
                    targetUser.UpdateRole(newRole);
                    
                    // Side-effect: Notification
                    try
                    {
                        await _notificationService.CreateForUserAsync(
                            targetUser.Id,
                            "Cập nhật vai trò",
                            $"Vai trò của bạn đã được quản trị viên thay đổi thành: {newRole.ToString()}.",
                            NotificationType.RoleChanged);
                    }
                    catch { }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ApplyForStaffAsync(Guid userId, ApplyStaffRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            dtc.Domain.ValueObjects.PhoneNumber? phoneObj = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                phoneObj = dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone);
            }

            user.UpdateProfile(
                fullName: request.FullName ?? user.FullName,
                phone: phoneObj,
                avatarUrl: user.AvatarUrl,
                updatedBy: userId
            );

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
