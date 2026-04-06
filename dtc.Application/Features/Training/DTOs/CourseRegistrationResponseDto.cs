using System;

namespace dtc.Application.Features.Training.DTOs
{
    public class CourseRegistrationResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalFee { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Extra User Info for Manager View
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Course Info
        public string? CourseName { get; set; }
        public string? LicenseTypeLabel { get; set; }

        // Image URLs from Document table
        public string? PhotoUrl { get; set; }
        public string? IdFrontUrl { get; set; }
        public string? IdBackUrl { get; set; }
    }
}
