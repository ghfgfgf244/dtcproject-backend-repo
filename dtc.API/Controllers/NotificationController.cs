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
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequestDto request)
        {
            if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var adminId))
                return Fail("Invalid token.");
            try
            {
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
            if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                return Fail("Invalid token.");

            var rolesClaim = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var userRoleIds = rolesClaim
                .Where(r => Enum.TryParse<dtc.Domain.Entities.UserRole>(r, out _))
                .Select(r => (int)Enum.Parse<dtc.Domain.Entities.UserRole>(r))
                .ToList();

            var notifications = await _notificationService.GetMyNotificationsAsync(userId, userRoleIds);
            return Ok(notifications);
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                return Fail("Invalid token.");
            try
            {
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
