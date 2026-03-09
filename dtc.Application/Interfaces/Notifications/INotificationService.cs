using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.DTOs.Notifications;

namespace dtc.Application.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> SendNotificationAsync(SendNotificationRequestDto request, Guid adminId);
        Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(Guid userId, List<int> userRoleIds);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
    }
}
