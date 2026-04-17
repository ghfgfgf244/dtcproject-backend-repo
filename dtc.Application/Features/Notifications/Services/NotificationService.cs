using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Notifications.DTOs;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Notifications;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Notifications.Services
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
            // 1. Get all read-receipts/links for this user to identify personal notifications
            var userReadReceipts = await _unitOfWork.UserNotifications
                .FindAsync(un => un.UserId == userId);
            
            var directNotificationIds = userReadReceipts.Select(un => un.NotificationId).ToHashSet();
            var userEnums = userRoleIds.Select(id => (dtc.Domain.Entities.UserRole)id).ToList();

            // 2. Query relevant notifications from MongoDB
            // Split into two separate queries because complex OR logic with nested Any/Contains 
            // can fail to translate in some MongoDB LINQ provider versions.
            
            // Query A: Notifications explicitly linked to this user
            var directNotifications = await _unitOfWork.Notifications
                .FindAsync(n => directNotificationIds.Contains(n.Id));

            // Query B: Shared/Broadcast notifications matching user roles
            var roleNotifications = await _unitOfWork.Notifications
                .FindAsync(n => n.TargetRoles.Any(role => userEnums.Contains(role)));

            // Combine and Deduplicate
            var relevantNotifications = directNotifications
                .Concat(roleNotifications)
                .GroupBy(n => n.Id)
                .Select(g => g.First())
                .ToList();

            var readNotificationIds = userReadReceipts.Where(un => un.IsRead).Select(un => un.NotificationId).ToHashSet();

            // 3. Map to DTOs
            return relevantNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Type = n.Type.ToString(),
                    CenterId = n.CenterId,
                    IsRead = readNotificationIds.Contains(n.Id),
                    CreatedAt = n.CreatedAt
                });
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetAllNotificationsAsync()
        {
            var allNotifications = await _unitOfWork.Notifications.FindAsync(n => true);
            return allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Type = n.Type.ToString(),
                    CenterId = n.CenterId,
                    IsRead = false, // Not relevant for admin-all view
                    CreatedAt = n.CreatedAt
                });
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

        public async Task CreateForUserAsync(
            Guid userId,
            string title,
            string content,
            NotificationType type,
            Guid? centerId = null)
        {
            // 1. Create notification document (no role filter = personal)
            var notification = new Notification(
                title: title,
                content: content,
                type: type,
                createdBy: userId,
                centerId: centerId,
                target: null // null = no role broadcast
            );
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            // 2. Create a personal read-receipt so this notification appears in the user's feed
            var receipt = new UserNotification(notification.Id, userId);
            await _unitOfWork.UserNotifications.AddAsync(receipt);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
