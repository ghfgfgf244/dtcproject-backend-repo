using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Exams
{
    public class UpdateExamRequestDto
    {
        [MaxLength(255)]
        public string? ExamName { get; set; }

        public DateTime? ExamDate { get; set; }

        public ExamType? ExamType { get; set; }

        [Range(1, 480)]
        public int? DurationMinutes { get; set; }

        [Range(1, 1000)]
        public int? TotalScore { get; set; }

        [Range(1, 1000)]
        public int? PassScore { get; set; }
        
        public ExamStatus? Status { get; set; }
    }
}
