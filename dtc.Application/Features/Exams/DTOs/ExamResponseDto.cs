using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using System;

namespace dtc.Application.Features.Exams.DTOs
{
    public class ExamResponseDto
    {
        public Guid Id { get; set; }
        public Guid ExamBatchId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public ExamType ExamType { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalScore { get; set; }
        public int PassScore { get; set; }
        public ExamStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
