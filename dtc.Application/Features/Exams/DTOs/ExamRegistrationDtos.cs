using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Application.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class CreateExamRegistrationRequestDto
    {
        [Required]
        public Guid ExamBatchId { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        public bool IsPaid { get; set; } = false;
    }

    public class UpdateExamRegistrationStatusRequestDto
    {
        [Required]
        public ExamRegistrationStatus Status { get; set; }
    }

    public class UpdateExamRegistrationPaymentRequestDto
    {
        [Required]
        public bool IsPaid { get; set; }
    }

    public class TermExamRegistrationCandidateDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string LicenseTypeLabel { get; set; } = string.Empty;
        public double AttendanceRate { get; set; }
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public bool IsEligibleForApproval { get; set; }
        public bool AlreadyRegistered { get; set; }
    }

    public class ExamRegistrationResponseDto
    {
        public Guid Id { get; set; }
        public Guid ExamBatchId { get; set; }
        public Guid StudentId { get; set; }
        public Guid? CenterId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string BatchName { get; set; } = string.Empty;
        public Guid? TermId { get; set; }
        public string? TermName { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string LicenseTypeLabel { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsPaid { get; set; }
        public double AttendanceRate { get; set; }
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public bool IsEligibleForApproval { get; set; }
        public ExamRegistrationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExamRegistrationBatchQueryDto : PagedRequest
    {
        public ExamRegistrationStatus? Status { get; set; }
    }

    public class ExamRegistrationBatchPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PendingCount { get; set; }
        public int EligibleCount { get; set; }
        public IEnumerable<ExamRegistrationResponseDto> Items { get; set; } = [];
    }
}
