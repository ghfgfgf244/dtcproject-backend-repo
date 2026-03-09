using dtc.Application.DTOs.Users;
using dtc.Application.Interfaces;
using dtc.Domain.Interfaces;
using System;
using System.Linq;
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

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Map string to PhoneNumber value object if provided
            dtc.Domain.ValueObjects.PhoneNumber? phoneObj = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                phoneObj = dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone);
            }

            // Call domain method to update
            bool isUpdated = user.UpdateProfile(
                fullName: request.FullName,
                phone: phoneObj,
                avatarUrl: null, // Avatar update could be handled separately or added to DTO if needed
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

            // Perform Soft Delete
            await _unitOfWork.Users.RemoveAsync(user); // EF Core override interceptor removes it via IsDeleted = true
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

            // We do not replace EF Core's IsDeleted here, just the IsActive property.
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<System.Collections.Generic.IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => true, u => u.Roles);
            
            var result = new System.Collections.Generic.List<UserResponseDto>();
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
            // 1. Check if email already exists
            var targetEmail = dtc.Domain.ValueObjects.Email.Create(request.Email);
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail);
            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            // 2. Hash Password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Create User Entity
            var newUser = new dtc.Domain.Entities.Permissions.User(
                email: targetEmail,
                passwordHash: passwordHash,
                fullName: request.FullName,
                phone: dtc.Domain.ValueObjects.PhoneNumber.Create(request.Phone),
                createdBy: adminId
            );

            // 4. Assign Roles if provided
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                // Verify the roles exist (they are enums so it's generally safe, but best to enforce valid IDs)
                foreach(var roleId in request.RoleIds)
                {
                    if (Enum.IsDefined(typeof(dtc.Domain.Entities.UserRole), roleId))
                    {
                        var roleEnum = (dtc.Domain.Entities.UserRole)roleId;
                        // Fetch the role entity so EF Core can track it
                        var roleEntity = await _unitOfWork.Roles.GetByIdAsync(roleId);
                        if (roleEntity != null)
                        {
                            newUser.AddRole(roleEntity);
                        }
                    }
                }
            }

            // 5. Save to database
            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            // 6. Return response
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
            // 1. Find User with Roles Included
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId, u => u.Roles);
            var targetUser = user.FirstOrDefault();

            if (targetUser == null)
            {
                throw new Exception("User not found.");
            }

            // 2. Prevent adding if already a Student to avoid implicit overhead
            if (targetUser.Roles.Any(r => r.Id == (int)dtc.Domain.Entities.UserRole.Student))
            {
                return; // Already a student
            }

            // 3. Fetch the Student Role entity (ID 6)
            var studentRole = await _unitOfWork.Roles.GetByIdAsync((int)dtc.Domain.Entities.UserRole.Student);
            if (studentRole == null)
            {
                throw new Exception("Student Role configuration missing in system.");
            }

            // 4. Assign the role
            targetUser.AddRole(studentRole);
            
            // 5. Save changes
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserRolesAsync(Guid targetUserId, UpdateUserRolesRequestDto request)
        {
            // 1. Find User with Roles
            var userQuery = await _unitOfWork.Users.FindAsync(u => u.Id == targetUserId, u => u.Roles);
            var targetUser = userQuery.FirstOrDefault();

            if (targetUser == null)
            {
                throw new Exception("User not found.");
            }

            // Prepare list of requested role IDs. Prevent granting Student role via this API if required by logic
            // Assuming DEV-56 says exclude Student role logic (Student role is 6).
            var requestedRoleIds = request.RoleIds.Distinct().ToList();
            if (requestedRoleIds.Contains((int)dtc.Domain.Entities.UserRole.Student))
            {
                throw new Exception("Cannot assign Student role through this endpoint. Use specific student registration flow.");
            }

            var currentRoleIds = targetUser.Roles.Select(r => r.Id).ToList();

            // Roles to Add
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

            // Roles to Remove: Only remove from the requested scope (meaning if they have Student role, DON'T remove it here)
            // The request.RoleIds should represent the COMPLETE list of their Staff roles. 
            // Any current staff role not in the requested list will be removed.
            var rolesToRemove = currentRoleIds
                .Where(id => id != (int)dtc.Domain.Entities.UserRole.Student) // Never remove Student role via this API
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

            // Optional: update basic info if provided
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

            // Here we would typically save the application to a tracking table.
            // For now, since we want to record the application state, we assume this info is logged 
            // or we just return success to indicate the application was submitted.
            // The actual role assignment will be done by Admin via DEV-56 (UpdateUserRolesAsync).

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

