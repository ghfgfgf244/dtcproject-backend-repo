using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dtc.Domain.Entities.Permissions;

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
                throw new KeyNotFoundException("User not found.");
            }

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Phone = user.Phone?.Value ?? string.Empty,
                AvatarUrl = user.AvatarUrl,
                RoleName = user.RoleId.ToString(),
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
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
                Phone = user.Phone?.Value ?? string.Empty,
                AvatarUrl = user.AvatarUrl,
                RoleName = user.RoleId.ToString(),
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }

        public async Task DeleteMyProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.SoftDelete(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleUserStatusAsync(Guid adminId, Guid targetUserId)
        {
            if (adminId == targetUserId)
            {
                throw new InvalidOperationException("You cannot ban or unban yourself.");
            }

            var targetUser = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            if (targetUser == null)
            {
                throw new KeyNotFoundException("Target user not found.");
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

        private async Task<List<UserResponseDto>> MapToUserResponseList(IEnumerable<User> users)
        {
            var result = new List<UserResponseDto>();
            var centers = await _unitOfWork.Centers.GetAllAsync();
            var userCenters = await _unitOfWork.UserCenters.GetAllAsync();

            foreach (var user in users)
            {
                var userCenter = userCenters.FirstOrDefault(uc => uc.UserId == user.Id);
                var center = userCenter != null ? centers.FirstOrDefault(c => c.Id == userCenter.CenterId) : null;

                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    FullName = user.FullName,
                    Phone = user.Phone?.Value ?? string.Empty,
                    AvatarUrl = user.AvatarUrl,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt,
                    Roles = new List<string> { user.RoleId.ToString() },
                    CenterId = center?.Id,
                    CenterName = center?.CenterName
                });
            }

            return result;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted);
            return await MapToUserResponseList(users);
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(int roleId)
        {
            var users = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted && (int)u.RoleId == roleId);
            return await MapToUserResponseList(users);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request, Guid adminId)
        {
            return await CreateManagedUserAsync(request, adminId, UserRole.Admin);
        }

        public async Task<UserResponseDto> CreateManagedUserAsync(CreateUserRequestDto request, Guid requesterId, UserRole requesterRole)
        {
            EnsureRequesterCanManageRole(requesterRole, request.RoleIds);

            var targetEmail = dtc.Domain.ValueObjects.Email.Create(request.Email);
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var clerkId = string.IsNullOrWhiteSpace(request.ClerkId)
                ? $"pending-{Guid.NewGuid():N}"
                : request.ClerkId;

            var newUser = new dtc.Domain.Entities.Permissions.User(
                clerkId: clerkId,
                email: targetEmail,
                fullName: request.FullName,
                phone: dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone),
                createdBy: requesterId
            );

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                var firstRoleId = request.RoleIds.First();
                if (Enum.IsDefined(typeof(dtc.Domain.Entities.UserRole), firstRoleId))
                {
                    newUser.UpdateRole((dtc.Domain.Entities.UserRole)firstRoleId);
                }
            }

            if (!request.IsActive)
            {
                newUser.Deactivate(requesterId);
            }

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _notificationService.CreateForUserAsync(
                    newUser.Id,
                    "Chao mung ban den voi DTC!",
                    "Tai khoan cua ban da duoc quan tri vien khoi tao thanh cong.",
                    NotificationType.Welcome);
            }
            catch
            {
            }

            return new UserResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email.Value,
                FullName = newUser.FullName,
                Phone = newUser.Phone?.Value ?? string.Empty,
                AvatarUrl = newUser.AvatarUrl,
                IsActive = newUser.IsActive,
                LastLoginAt = newUser.LastLoginAt,
                Roles = new List<string> { newUser.RoleId.ToString() }
            };
        }

        public async Task<UserResponseDto> UpdateManagedUserAsync(Guid targetUserId, UpdateManagedUserRequestDto request, Guid requesterId, UserRole requesterRole)
        {
            var targetUser = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            if (targetUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            EnsureRequesterCanManageExistingUser(requesterRole, targetUser.RoleId);

            dtc.Domain.ValueObjects.PhoneNumber? phoneObj = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                phoneObj = dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone);
            }

            targetUser.UpdateProfile(
                fullName: request.FullName,
                phone: phoneObj,
                avatarUrl: targetUser.AvatarUrl,
                updatedBy: requesterId);

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                {
                    targetUser.Activate(requesterId);
                }
                else
                {
                    targetUser.Deactivate(requesterId);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return (await MapToUserResponseList(new[] { targetUser })).First();
        }

        public async Task ToggleManagedUserStatusAsync(Guid targetUserId, Guid requesterId, UserRole requesterRole)
        {
            var targetUser = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            if (targetUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            EnsureRequesterCanManageExistingUser(requesterRole, targetUser.RoleId);

            if (targetUser.IsActive)
            {
                targetUser.Deactivate(requesterId);
            }
            else
            {
                targetUser.Activate(requesterId);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteManagedUserAsync(Guid targetUserId, Guid requesterId, UserRole requesterRole)
        {
            var targetUser = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            if (targetUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            EnsureRequesterCanManageExistingUser(requesterRole, targetUser.RoleId);
            targetUser.SoftDelete(requesterId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddStudentRoleAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId);
            var targetUser = user.FirstOrDefault();

            if (targetUser == null)
            {
                throw new KeyNotFoundException("User not found.");
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
                throw new KeyNotFoundException("User not found.");
            }

            var requestedRoleIds = request.RoleIds.Distinct().ToList();

            if (requestedRoleIds.Any())
            {
                var newRoleId = requestedRoleIds.First();
                if (Enum.IsDefined(typeof(dtc.Domain.Entities.UserRole), newRoleId))
                {
                    var newRole = (dtc.Domain.Entities.UserRole)newRoleId;
                    targetUser.UpdateRole(newRole);

                    try
                    {
                        await _notificationService.CreateForUserAsync(
                            targetUser.Id,
                            "Cap nhat vai tro",
                            $"Vai tro cua ban da duoc quan tri vien thay doi thanh: {newRole}.",
                            NotificationType.RoleChanged);
                    }
                    catch
                    {
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ApplyForStaffAsync(Guid userId, ApplyStaffRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
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

        public async Task DeleteUserAsync(Guid targetUserId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.SoftDelete();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserStatsDto> GetUserStatsAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => !u.IsDeleted);
            
            return new UserStatsDto
            {
                TotalUsers = users.Count(),
                StaffCount = users.Count(u => u.RoleId == UserRole.TrainingManager || u.RoleId == UserRole.EnrollmentManager),
                InstructorCount = users.Count(u => u.RoleId == UserRole.Instructor),
                CollaboratorCount = users.Count(u => u.RoleId == UserRole.Collaborator),
                StudentCount = users.Count(u => u.RoleId == UserRole.Student)
            };
        }

        public async Task AssignCenterAsync(Guid userId, Guid centerId, Guid adminId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            var center = await _unitOfWork.Centers.GetByIdAsync(centerId);
            if (center == null) throw new KeyNotFoundException("Center not found");

            var currentAssociations = await _unitOfWork.UserCenters.FindAsync(uc => uc.UserId == userId);
            var toRemove = currentAssociations.ToList();

            if (toRemove.Any())
            {
                await _unitOfWork.UserCenters.RemoveRange(toRemove);
            }

            await _unitOfWork.UserCenters.AddAsync(new UserCenter(userId, centerId));
            await _unitOfWork.SaveChangesAsync();
        }

        private static void EnsureRequesterCanManageRole(UserRole requesterRole, List<int>? roleIds)
        {
            if (roleIds == null || roleIds.Count == 0)
            {
                return;
            }

            var requestedRoleId = roleIds.First();
            if (!Enum.IsDefined(typeof(UserRole), requestedRoleId))
            {
                throw new InvalidOperationException("Invalid role.");
            }

            EnsureRequesterCanManageExistingUser(requesterRole, (UserRole)requestedRoleId);
        }

        private static void EnsureRequesterCanManageExistingUser(UserRole requesterRole, UserRole targetRole)
        {
            if (requesterRole == UserRole.Admin)
            {
                return;
            }

            if (requesterRole == UserRole.TrainingManager && targetRole == UserRole.Instructor)
            {
                return;
            }

            if (requesterRole == UserRole.EnrollmentManager && targetRole == UserRole.Collaborator)
            {
                return;
            }

            throw new InvalidOperationException("You do not have permission to manage this user role.");
        }
    }
}
