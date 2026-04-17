using dtc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class CreateQuestionRequestDto
    {
        [Required]
        public string Category { get; set; } = QuestionCategoryNames.Theory;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? AnswerA { get; set; }
        public string? AnswerB { get; set; }
        public string? AnswerC { get; set; }
        public string? AnswerD { get; set; }

        [Required]
        public AnswerOption CorrectAnswer { get; set; }

        public string? ImageLink { get; set; }
        public string? Explanation { get; set; }
    }

    public class UpdateQuestionRequestDto
    {
        [Required]
        public string Category { get; set; } = QuestionCategoryNames.Theory;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? AnswerA { get; set; }
        public string? AnswerB { get; set; }
        public string? AnswerC { get; set; }
        public string? AnswerD { get; set; }

        [Required]
        public AnswerOption CorrectAnswer { get; set; }

        public string? ImageLink { get; set; }
        public string? Explanation { get; set; }
    }

    public class QuestionResponseDto
    {
        public int Id { get; set; }
        public int? Order { get; set; }
        public string Category { get; set; } = QuestionCategoryNames.Theory;
        public string Content { get; set; } = string.Empty;
        public string? AnswerA { get; set; }
        public string? AnswerB { get; set; }
        public string? AnswerC { get; set; }
        public string? AnswerD { get; set; }
        public AnswerOption CorrectAnswer { get; set; }
        public string? ImageLink { get; set; }
        public string? Explanation { get; set; }
        public int AttemptCount { get; set; }
        public int WrongAttemptCount { get; set; }
        public double WrongRate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommonMistakeQuestionDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = QuestionCategoryNames.Theory;
        public string Content { get; set; } = string.Empty;
        public string? ImageLink { get; set; }
        public string? Explanation { get; set; }
        public int AttemptCount { get; set; }
        public int WrongAttemptCount { get; set; }
        public double WrongRate { get; set; }
    }

    public class QuestionImportResponseDto
    {
        public int ImportedCount { get; set; }
        public List<string> Warnings { get; set; } = new();
        public List<QuestionResponseDto> Questions { get; set; } = new();
    }
}
