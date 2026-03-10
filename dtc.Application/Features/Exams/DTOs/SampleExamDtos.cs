using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class CreateSampleExamRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public int ExamNo { get; set; }

        [Required]
        public ExamLevel Level { get; set; }

        [Required]
        [Range(1, 1000)]
        public int DurationMinutes { get; set; }

        [Required]
        public int PassingScore { get; set; }
    }

    public class SampleExamQuestionDto
    {
        public int QuestionId { get; set; }
        public int Order { get; set; }
    }

    public class UpdateSampleExamQuestionsRequestDto
    {
        [Required]
        public List<SampleExamQuestionDto> Questions { get; set; } = new();
    }

    public class SampleExamResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public int ExamNo { get; set; }
        public ExamLevel Level { get; set; }
        public int DurationMinutes { get; set; }
        public int PassingScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalQuestions { get; set; }
    }

    public class SampleExamDetailResponseDto : SampleExamResponseDto
    {
        public List<QuestionResponseDto> Questions { get; set; } = new();
    }

    public class SubmitSampleTestRequestDto
    {
        [Required]
        public int DurationSeconds { get; set; }

        [Required]
        public Dictionary<int, string> Answers { get; set; } = new(); // QuestionId -> Answer string (A, B, C, D)
    }

    public class SampleTestResultResponseDto
    {
        public Guid ResultId { get; set; }
        public double TotalScore { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsPassed { get; set; }
        public Dictionary<int, string> CorrectAnswers { get; set; } = new();
    }
}
