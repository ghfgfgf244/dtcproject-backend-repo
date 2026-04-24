using System;

namespace dtc.Application.Features.Training.DTOs
{
    public class TermResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid CenterId { get; set; }
        public string? CourseName { get; set; }
        public string? CenterName { get; set; }
        public string TermName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
