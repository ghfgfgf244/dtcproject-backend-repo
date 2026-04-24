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
        public decimal OriginalFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Extra User Info for Manager View
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Course Info
        public Guid? CenterId { get; set; }
        public string? CenterName { get; set; }
        public string? CourseName { get; set; }
        public string? LicenseTypeLabel { get; set; }
        public Guid? AssignedTermId { get; set; }
        public string? AssignedTermName { get; set; }
        public Guid? AssignedClassId { get; set; }
        public string? AssignedClassName { get; set; }
        public Guid? SuggestedTermId { get; set; }
        public string? SuggestedTermName { get; set; }
        public DateTime? SuggestedTermStartDate { get; set; }
        public string? PlacementMessage { get; set; }
        public string? AppliedReferralCode { get; set; }
        public string? AppliedReferralCollaboratorName { get; set; }

        // Image URLs from Document table
        public string? PhotoUrl { get; set; }
        public string? IdFrontUrl { get; set; }
        public string? IdBackUrl { get; set; }
    }
}
