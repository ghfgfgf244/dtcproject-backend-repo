using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class CreateSampleExamWithQuestionsRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public int ExamNo { get; set; }

        public dtc.Domain.Entities.ExamLevel Level { get; set; } = dtc.Domain.Entities.ExamLevel.A1;

        [Range(1, 120)]
        public int DurationMinutes { get; set; } = 20;

        [Range(0, 100)]
        public int PassingScore { get; set; } = 80;

        public List<SampleQuestionItemDto> Questions { get; set; } = new();
    }

    public class SampleQuestionItemDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
