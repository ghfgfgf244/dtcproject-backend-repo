using System;
using dtc.Domain.Entities;

namespace dtc.Application.Features.Notifications.DTOs
{
    public class NotificationResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid? CenterId { get; set; }
        
        // This is dynamic based on user accessing it
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
