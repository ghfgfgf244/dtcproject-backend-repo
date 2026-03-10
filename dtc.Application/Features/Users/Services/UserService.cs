using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
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
            var users = await _unitOfWork.Users.FindAsync(u => true, u => u.Roles);
            
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
                    Roles = user.Roles.Select(r => r.RoleName.ToString()).ToList()
                });
            }

            return result;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(int roleId)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Roles.Any(r => (int)r.RoleName == roleId), u => u.Roles);
            
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
                    Roles = user.Roles.Select(r => r.RoleName.ToString()).ToList()
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

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new dtc.Domain.Entities.Permissions.User(
                email: targetEmail,
                passwordHash: passwordHash,
                fullName: request.FullName,
                phone: dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone),
                createdBy: adminId
            );

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                foreach(var roleId in request.RoleIds)
                {
                    if (Enum.IsDefined(typeof(dtc.Domain.Entities.UserRole), roleId))
                    {
                        var roleEntity = await _unitOfWork.Roles.GetByIdAsync(roleId);
                        if (roleEntity != null)
                        {
                            newUser.AddRole(roleEntity);
                        }
                    }
                }
            }

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email.Value,
                FullName = newUser.FullName,
                Phone = newUser.Phone?.Value ?? "",
                AvatarUrl = newUser.AvatarUrl,
                IsActive = newUser.IsActive,
                LastLoginAt = newUser.LastLoginAt,
                Roles = newUser.Roles.Select(r => r.RoleName.ToString()).ToList()
            };
        }

        public async Task AddStudentRoleAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId, u => u.Roles);
            var targetUser = user.FirstOrDefault();

            if (targetUser == null)
            {
                throw new Exception("User not found.");
            }

            if (targetUser.Roles.Any(r => r.Id == (int)dtc.Domain.Entities.UserRole.Student))
            {
                return;
            }

            var studentRole = await _unitOfWork.Roles.GetByIdAsync((int)dtc.Domain.Entities.UserRole.Student);
            if (studentRole == null)
            {
                throw new Exception("Student Role configuration missing in system.");
            }

            targetUser.AddRole(studentRole);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserRolesAsync(Guid targetUserId, UpdateUserRolesRequestDto request)
        {
            var userQuery = await _unitOfWork.Users.FindAsync(u => u.Id == targetUserId, u => u.Roles);
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

            var currentRoleIds = targetUser.Roles.Select(r => r.Id).ToList();

            var rolesToAdd = requestedRoleIds.Except(currentRoleIds).ToList();
            foreach (var roleId in rolesToAdd)
            {
                if (Enum.IsDefined(typeof(dtc.Domain.Entities.UserRole), roleId))
                {
                    var roleEntity = await _unitOfWork.Roles.GetByIdAsync(roleId);
                    if (roleEntity != null)
                    {
                        targetUser.AddRole(roleEntity);
                    }
                }
            }

            var rolesToRemove = currentRoleIds
                .Where(id => id != (int)dtc.Domain.Entities.UserRole.Student)
                .Except(requestedRoleIds)
                .ToList();
                
            foreach (var roleId in rolesToRemove)
            {
                var roleToRemove = targetUser.Roles.FirstOrDefault(r => r.Id == roleId);
                if (roleToRemove != null)
                {
                    targetUser.RemoveRole(roleToRemove);
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
