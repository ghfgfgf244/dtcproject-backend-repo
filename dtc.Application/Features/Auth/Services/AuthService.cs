using dtc.Application.Features.Auth.DTOs;
using dtc.Application.Features.Auth.Interfaces;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces;
using dtc.Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using EmailVO = dtc.Domain.ValueObjects.Email;

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
            var fullName = ResolveFullName(request.FullName, request.Email);
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.ClerkId == request.ClerkId);
            var shouldSendWelcomeNotification = false;

            if (user == null)
            {
                // Check if a matching user already exists locally but hasn't been linked to Clerk yet.
                user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail);
                if (user != null)
                {
                    user.SyncFromClerk(request.ClerkId, fullName, request.AvatarUrl);
                }
                else
                {
                    user = new User(
                        clerkId: request.ClerkId,
                        email: targetEmail,
                        fullName: fullName,
                        phone: PhoneNumber.Create(request.Phone ?? "0000000000"));

                    await _unitOfWork.Users.AddAsync(user);
                    shouldSendWelcomeNotification = true;
                }
            }
            else
            {
                user.SyncFromClerk(request.ClerkId, fullName, request.AvatarUrl);
            }

            if (!string.IsNullOrWhiteSpace(request.Role) &&
                Enum.TryParse<UserRole>(request.Role, true, out var clerkRole))
            {
                user.UpdateRole(clerkRole);
            }

            if (request.CenterId.HasValue && request.CenterId.Value != Guid.Empty)
            {
                var existingLink = await _unitOfWork.UserCenters.FirstOrDefaultAsync(
                    uc => uc.UserId == user.Id && uc.CenterId == request.CenterId.Value);

                if (existingLink == null)
                {
                    var userCenter = new UserCenter(user.Id, request.CenterId.Value);
                    await _unitOfWork.UserCenters.AddAsync(userCenter);
                }
            }

            user.UpdateLastLogin();
            await _unitOfWork.SaveChangesAsync();

            if (shouldSendWelcomeNotification)
            {
                try
                {
                    await _notificationService.CreateForUserAsync(
                        user.Id,
                        "Ch\u00e0o m\u1eebng b\u1ea1n \u0111\u1ebfn v\u1edbi DTC!",
                        $"Xin ch\u00e0o {user.FullName}, t\u00e0i kho\u1ea3n c\u1ee7a b\u1ea1n \u0111\u00e3 \u0111\u01b0\u1ee3c k\u1ebft n\u1ed1i th\u00e0nh c\u00f4ng.",
                        NotificationType.Welcome);
                }
                catch
                {
                }
            }

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Token = ""
            };
        }

        private static string ResolveFullName(string? fullName, string email)
        {
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }

            var emailPrefix = email.Split('@', 2, StringSplitOptions.TrimEntries)[0];
            return string.IsNullOrWhiteSpace(emailPrefix) ? "DTC User" : emailPrefix;
        }
    }
}
