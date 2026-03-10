using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Exams
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

    public class ExamRegistrationResponseDto
    {
        public Guid Id { get; set; }
        public Guid ExamBatchId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string BatchName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsPaid { get; set; }
        public ExamRegistrationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
