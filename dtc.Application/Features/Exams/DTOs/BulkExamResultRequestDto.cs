using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class BulkExamResultRequestDto
    {
        [Required]
        public Guid ExamId { get; set; }

        [Required]
        public List<StudentScoreDto> Results { get; set; } = new();
    }

    public class StudentScoreDto
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [Range(0, 100)]
        public double Score { get; set; }
    }
}
