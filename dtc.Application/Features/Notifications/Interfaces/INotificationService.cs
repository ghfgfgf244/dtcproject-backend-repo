using dtc.Application.Features.Notifications.DTOs;
using dtc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> SendNotificationAsync(SendNotificationRequestDto request, Guid adminId);
        Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(Guid userId, List<int> userRoleIds);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);

        /// <summary>
        /// Tạo thông báo gửi trực tiếp cho 1 user cụ thể (không broadcast theo role).
        /// Dùng nội bộ trong các Application Service.
        /// </summary>
        Task CreateForUserAsync(Guid userId, string title, string content, NotificationType type, Guid? centerId = null);
    }
}
