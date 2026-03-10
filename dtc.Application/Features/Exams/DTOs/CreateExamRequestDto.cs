using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class CreateExamRequestDto
    {
        [Required]
        public Guid ExamBatchId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ExamName { get; set; } = string.Empty;

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public ExamType ExamType { get; set; }

        [Required]
        [Range(1, 480)]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(1, 1000)]
        public int TotalScore { get; set; }

        [Required]
        [Range(1, 1000)]
        public int PassScore { get; set; }
    }
}
