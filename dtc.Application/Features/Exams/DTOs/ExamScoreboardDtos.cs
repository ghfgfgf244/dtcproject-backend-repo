using dtc.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Exams.DTOs
{
    public class ExamScoreboardQueryDto
    {
        public Guid? CourseId { get; set; }
        public Guid? TermId { get; set; }
        public Guid? ExamBatchId { get; set; }
        public string? Search { get; set; }
        public string SortDirection { get; set; } = "desc";
        [Range(1, 1000)]
        public int Page { get; set; } = 1;
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }

    public class UpsertStudentExamScoresRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid TermId { get; set; }

        [Required]
        public Guid ExamBatchId { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        public double? TheoryScore { get; set; }
        public double? PracticeScore { get; set; }
        public double? SimulationScore { get; set; }
    }

    public class ExamScoreImportRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid TermId { get; set; }

        [Required]
        public Guid ExamBatchId { get; set; }
    }

    public class ExamScoreboardResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int TotalPassed { get; set; }
        public decimal AverageOverallScore { get; set; }
        public bool HasSimulationExam { get; set; }
        public List<ExamScoreboardItemDto> Items { get; set; } = new();
    }

    public class ExamScoreboardItemDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string LicenseTypeLabel { get; set; } = string.Empty;
        public Guid TermId { get; set; }
        public string TermName { get; set; } = string.Empty;
        public Guid ExamBatchId { get; set; }
        public string ExamBatchName { get; set; } = string.Empty;
        public bool HasSimulationExam { get; set; }
        public double? TheoryScore { get; set; }
        public double? PracticeScore { get; set; }
        public double? SimulationScore { get; set; }
        public bool TheoryPassed { get; set; }
        public bool PracticePassed { get; set; }
        public bool? SimulationPassed { get; set; }
        public decimal OverallScore { get; set; }
        public bool IsPassedAll { get; set; }
        public int CompletedComponents { get; set; }
        public int TotalComponents { get; set; }
    }

    public class ExamScoreImportResponseDto
    {
        public int ImportedCount { get; set; }
        public List<string> Warnings { get; set; } = new();
    }
}
