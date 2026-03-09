using dtc.Application.DTOs.Notifications;
using dtc.Application.Interfaces.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
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
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim == null || !Guid.TryParse(adminIdClaim.Value, out var adminId))
            {
                return Unauthorized(new { Error = "Invalid token." });
            }

            try
            {
                var response = await _notificationService.SendNotificationAsync(request, adminId);
                return CreatedAtAction(nameof(GetMyNotifications), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Error = "Invalid token." });
            }

            var rolesClaim = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var userRoleIds = new System.Collections.Generic.List<int>();

            foreach (var roleName in rolesClaim)
            {
                if (Enum.TryParse<dtc.Domain.Entities.UserRole>(roleName, out var roleEnum))
                {
                    userRoleIds.Add((int)roleEnum);
                }
            }

            try
            {
                var notifications = await _notificationService.GetMyNotificationsAsync(userId, userRoleIds);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Error = "Invalid token." });
            }

            try
            {
                await _notificationService.MarkAsReadAsync(id, userId);
                return Ok(new { Message = "Notification marked as read." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
