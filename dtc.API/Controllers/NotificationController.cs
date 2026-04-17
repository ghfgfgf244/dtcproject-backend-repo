using dtc.Application.Features.Notifications.DTOs;
using dtc.Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequestDto request)
        {
            try
            {
                var adminId = await GetInternalUserIdAsync();
                var response = await _notificationService.SendNotificationAsync(request, adminId);
                return Created(response, "Notification sent.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = await GetInternalUserIdAsync();
            var rolesClaim = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var userRoleIds = rolesClaim
                .Where(r => Enum.TryParse<dtc.Domain.Entities.UserRole>(r, out _))
                .Select(r => (int)Enum.Parse<dtc.Domain.Entities.UserRole>(r))
                .ToList();

            var notifications = await _notificationService.GetMyNotificationsAsync(userId, userRoleIds);
            return Ok(notifications);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var userId = await GetInternalUserIdAsync();
                await _notificationService.MarkAsReadAsync(id, userId);
                return NoContent("Notification marked as read.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
