using dtc.Application.Features.Auth.Interfaces;
using dtc.Application.Features.Auth.DTOs;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Interfaces;
using dtc.Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using EmailVO = dtc.Domain.ValueObjects.Email;
using dtc.Domain.Entities.Permissions;

namespace dtc.Application.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public AuthService(
            IUnitOfWork unitOfWork,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _notificationService = notificationService;
        }

        public async Task<AuthResponseDto> SyncUserAsync(SyncUserRequestDto request)
        {
            var targetEmail = EmailVO.Create(request.Email);
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.ClerkId == request.ClerkId);

            if (user == null)
            {
                // check if user with same email exists (might have been created by admin without clerkId)
                user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail);
                if (user != null)
                {
                    // link existing user to ClerkId
                    // We need a method in User to set ClerkId if not already set, 
                    // or just use reflection/private setter if we want to be strict.
                    // For now, I'll add a SyncFromClerk that also sets ClerkId if missing.
                    user.SyncFromClerk(request.FullName, request.AvatarUrl);
                    // I'll add a SetClerkId method to User that's public for this sync purpose.
                }
                else
                {
                    // Create new user
                    user = new User(
                        clerkId: request.ClerkId,
                        email: targetEmail,
                        fullName: request.FullName,
                        phone: PhoneNumber.Create(request.Phone ?? "0000000000") // default phone
                    );
                    await _unitOfWork.Users.AddAsync(user);

                    // Side-effect: welcome notification
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _notificationService.CreateForUserAsync(
                                user.Id,
                                "Chào mừng bạn đến với DTC!",
                                $"Xin chào {user.FullName}, tài khoản của bạn đã được kết nối thành công.",
                                NotificationType.Welcome);
                        }
                        catch { }
                    });
                }
            }
            else
            {
                // Update existing user info from Clerk if changed
                user.SyncFromClerk(request.FullName, request.AvatarUrl);
            }

            // Sync Role from Metadata if provided
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                if (Enum.TryParse<UserRole>(request.Role, true, out var clerkRole))
                {
                    user.UpdateRole(clerkRole);
                }
            }

            // Sync Center from Metadata if provided
            if (request.CenterId.HasValue && request.CenterId.Value != Guid.Empty)
            {
                var existingLink = await _unitOfWork.UserCenters.FirstOrDefaultAsync(
                    uc => uc.UserId == user.Id && uc.CenterId == request.CenterId.Value);
                
                if (existingLink == null)
                {
                    var userCenter = new dtc.Domain.Entities.Permissions.UserCenter(user.Id, request.CenterId.Value);
                    await _unitOfWork.UserCenters.AddAsync(userCenter);
                }
            }

            user.UpdateLastLogin();
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Token = "" // Token is managed by Clerk on the frontend
            };
        }
    }
}
