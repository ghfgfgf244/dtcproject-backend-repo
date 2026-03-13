using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Notifications.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Notifications.DTOs;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Notifications.DTOs;

namespace dtc.Application.Features.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> SendNotificationAsync(SendNotificationRequestDto request, Guid adminId);
        Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(Guid userId, List<int> userRoleIds);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
    }
}
