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

    public class PublicSampleExamDetailResponseDto : SampleExamResponseDto
    {
        public List<PublicQuestionResponseDto> Questions { get; set; } = new();
    }

    public class PublicQuestionResponseDto
    {
        public int Id { get; set; }
        public int? Order { get; set; }
        public string Category { get; set; } = QuestionCategoryNames.Theory;
        public string Content { get; set; } = string.Empty;
        public string? AnswerA { get; set; }
        public string? AnswerB { get; set; }
        public string? AnswerC { get; set; }
        public string? AnswerD { get; set; }
        public string? ImageLink { get; set; }
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
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public Dictionary<int, string> CorrectAnswers { get; set; } = new();
        public Dictionary<int, string> Explanations { get; set; } = new();
        public List<SampleExamQuestionReviewDto> ReviewItems { get; set; } = new();
        public SampleExamInsightDto? Insight { get; set; }
    }

    public class SampleExamQuestionReviewDto
    {
        public int QuestionId { get; set; }
        public string Category { get; set; } = QuestionCategoryNames.Theory;
        public bool IsCorrect { get; set; }
        public string? SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string StudyTip { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public int WrongAttemptCount { get; set; }
        public double WrongRate { get; set; }
    }

    public class SampleExamInsightDto
    {
        public string Summary { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public Dictionary<string, int> WrongCountsByCategory { get; set; } = new();
        public List<string> SuggestedTopics { get; set; } = new();
    }
}
