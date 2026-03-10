using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Exams
{
    public class CreateQuestionRequestDto
    {
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
        public string Content { get; set; } = string.Empty;

        public string? AnswerA { get; set; }
        public string? AnswerB { get; set; }
        public string? AnswerC { get; set; }
        public string? AnswerD { get; set; }

        [Required]
        public AnswerOption CorrectAnswer { get; set; }

        public string? Explanation { get; set; }
    }

    public class QuestionResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? AnswerA { get; set; }
        public string? AnswerB { get; set; }
        public string? AnswerC { get; set; }
        public string? AnswerD { get; set; }
        public AnswerOption CorrectAnswer { get; set; }
        public string? ImageLink { get; set; }
        public string? Explanation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
