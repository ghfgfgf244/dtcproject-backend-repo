using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using dtc.Application.Features.Users.Interfaces;
using dtc.Application.Features.Users.DTOs;
using System;
using System.Collections.Generic;

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
}
