using System;

namespace dtc.Application.Features.Collaborators.DTOs
{
    public class ReferralCodeResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
        public decimal CommissionRate { get; set; } // Standard return
    }

    public class CreateReferralCodeRequestDto
    {
        public string Code { get; set; } = string.Empty;
    }

    public class CollaboratorCommissionResponseDto
    {
        public Guid Id { get; set; }
        public Guid CollaboratorId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class CollaboratorAdminResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ReferralCode { get; set; } = string.Empty;
        public int UsedCount { get; set; }
        public bool IsCodeActive { get; set; }
        public decimal TotalPendingCommission { get; set; }
        public decimal TotalPaidCommission { get; set; }
    }

    public class CommissionAdminResponseDto
    {
        public Guid Id { get; set; }
        public Guid CollaboratorId { get; set; }
        public string CollaboratorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class CollaboratorAdminStatsDto
    {
        public int TotalCollaborators { get; set; }
        public decimal TotalCommissions { get; set; }
        public decimal PaidCommissions { get; set; }
        public decimal UnpaidCommissions { get; set; }
    }
}
