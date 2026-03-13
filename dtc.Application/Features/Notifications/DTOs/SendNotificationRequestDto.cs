using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using dtc.Domain.Entities;

namespace dtc.Application.Features.Notifications.DTOs
{
    public class SendNotificationRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required")]
        public NotificationType Type { get; set; }

        public Guid? CenterId { get; set; }

        // Optional list of Target Roles. If empty, it's a broadcast to everyone.
        public List<UserRole>? TargetRoles { get; set; }
    }
}
