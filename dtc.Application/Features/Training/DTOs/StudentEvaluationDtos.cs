using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class CreateStudentEvaluationRequestDto
    {
        [Required]
        public Guid StudentId { get; set; }

        public Guid? ClassId { get; set; }

        [Range(1, 10)]
        public int PunctualityScore { get; set; }

        [Range(1, 10)]
        public int SkillLevel { get; set; }

        public string Note { get; set; } = string.Empty;
    }

    public class UpdateStudentEvaluationRequestDto
    {
        [Range(1, 10)]
        public int PunctualityScore { get; set; }

        [Range(1, 10)]
        public int SkillLevel { get; set; }

        public string Note { get; set; } = string.Empty;
    }

    public class StudentEvaluationResponseDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public Guid? ClassId { get; set; }
        public int PunctualityScore { get; set; }
        public int SkillLevel { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime EvaluationDate { get; set; }
    }
}
