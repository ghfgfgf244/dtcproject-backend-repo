using dtc.Application.DTOs.Notifications;
using dtc.Application.Interfaces.Notifications;
using dtc.Domain.Entities.Notifications;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<NotificationResponseDto> SendNotificationAsync(SendNotificationRequestDto request, Guid adminId)
        {
            var notification = new Notification(
                title: request.Title,
                content: request.Content,
                type: request.Type,
                createdBy: adminId,
                centerId: request.CenterId,
                target: request.TargetRoles
            );

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync(); // Assuming Mongo repo supports SaveChangesAsync or AddAsync auto-saves

            return new NotificationResponseDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Content = notification.Content,
                Type = notification.Type.ToString(),
                CenterId = notification.CenterId,
                IsRead = false,
                CreatedAt = notification.CreatedAt
            };
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(Guid userId, List<int> userRoleIds)
        {
            // 1. Get all notifications relevant to the user's roles (or broadcast/empty roles)
            var allNotifications = await _unitOfWork.Notifications.FindAsync(n => true); // In a real app, optimize this query or get latest

            var userEnums = userRoleIds.Select(id => (dtc.Domain.Entities.UserRole)id).ToList();

            var relevantNotifications = allNotifications
                .Where(n => !n.TargetRoles.Any() || n.TargetRoles.Any(tr => userEnums.Contains(tr.Role)))
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            // 2. Get the read receipts for this user
            var userReadReceipts = await _unitOfWork.UserNotifications
                .FindAsync(un => un.UserId == userId);
            
            var readNotificationIds = userReadReceipts.Where(un => un.IsRead).Select(un => un.NotificationId).ToHashSet();

            // 3. Map to DTOs
            var response = relevantNotifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type.ToString(),
                CenterId = n.CenterId,
                IsRead = readNotificationIds.Contains(n.Id),
                CreatedAt = n.CreatedAt
            });

            return response;
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            // First check if a receipt already exists
            var existingReceipts = await _unitOfWork.UserNotifications.FindAsync(un => un.NotificationId == notificationId && un.UserId == userId);
            var receipt = existingReceipts.FirstOrDefault();

            if (receipt == null)
            {
                // Create new receipt marked as read
                receipt = new UserNotification(notificationId, userId);
                receipt.MarkAsRead();
                await _unitOfWork.UserNotifications.AddAsync(receipt);
            }
            else
            {
                if (!receipt.IsRead)
                {
                    receipt.MarkAsRead();
                    await _unitOfWork.UserNotifications.UpdateAsync(receipt);
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
