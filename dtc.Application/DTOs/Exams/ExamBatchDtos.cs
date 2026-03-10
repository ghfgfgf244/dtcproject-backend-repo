using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Exams
{
    public class CreateExamBatchRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string BatchName { get; set; } = string.Empty;

        [Required]
        public DateTime RegistrationStartDate { get; set; }

        [Required]
        public DateTime RegistrationEndDate { get; set; }

        [Required]
        public DateTime ExamStartDate { get; set; }
    }

    public class UpdateExamBatchRequestDto
    {
        [MaxLength(255)]
        public string? BatchName { get; set; }

        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public DateTime? ExamStartDate { get; set; }
    }

    public class UpdateExamBatchStatusRequestDto
    {
        [Required]
        public ExamBatchStatus Status { get; set; }
    }

    public class ExamBatchResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string BatchName { get; set; } = string.Empty;
        public DateTime RegistrationStartDate { get; set; }
        public DateTime RegistrationEndDate { get; set; }
        public DateTime ExamStartDate { get; set; }
        public ExamBatchStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
