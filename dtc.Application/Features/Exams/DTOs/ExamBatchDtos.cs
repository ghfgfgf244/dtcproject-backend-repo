using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;
using System.ComponentModel.DataAnnotations;
using dtc.Application.Common;

namespace dtc.Application.Features.Exams.DTOs
{
    public class ExamBatchPagedQueryDto : PagedRequest
    {
        public string? Keyword { get; set; }
        public ExamBatchStatus? Status { get; set; }
        public ExamBatchScopeType? ScopeType { get; set; }
    }

    public class CreateExamBatchRequestDto
    {
        public ExamBatchScopeType ScopeType { get; set; } = ExamBatchScopeType.Center;

        public Guid? CenterId { get; set; }

        [Required]
        [MaxLength(255)]
        public string BatchName { get; set; } = string.Empty;

        [Required]
        public DateTime RegistrationStartDate { get; set; }

        [Required]
        public DateTime RegistrationEndDate { get; set; }

        [Required]
        public DateTime ExamStartDate { get; set; }

        public int MaxCandidates { get; set; }
    }

    public class UpdateExamBatchRequestDto
    {
        public ExamBatchScopeType? ScopeType { get; set; }
        public Guid? CenterId { get; set; }

        [MaxLength(255)]
        public string? BatchName { get; set; }

        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public DateTime? ExamStartDate { get; set; }
        public int? MaxCandidates { get; set; }
        public ExamBatchStatus? Status { get; set; }
    }

    public class UpdateExamBatchStatusRequestDto
    {
        [Required]
        public ExamBatchStatus Status { get; set; }
    }

    public class ExamBatchResponseDto
    {
        public Guid Id { get; set; }
        public ExamBatchScopeType ScopeType { get; set; }
        public Guid? CenterId { get; set; }
        public string? CenterName { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public DateTime RegistrationStartDate { get; set; }
        public DateTime RegistrationEndDate { get; set; }
        public DateTime ExamStartDate { get; set; }
        public int CurrentCandidates { get; set; }
        public int MaxCandidates { get; set; }
        public ExamBatchStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExamBatchPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PendingItems { get; set; }
        public int ApprovedItems { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalCapacity { get; set; }
        public List<ExamBatchResponseDto> Items { get; set; } = new();
    }
}
