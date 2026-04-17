using System;

namespace dtc.Application.Features.Training.DTOs
{
    public class ClassResponseDto
    {
        public Guid Id { get; set; }
        public Guid TermId { get; set; }
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int CurrentStudents { get; set; }
        public int MaxStudents { get; set; }
        public string ClassType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TermName { get; set; }
        public string? CourseName { get; set; }
        public string? InstructorName { get; set; }
        public Guid? CenterId { get; set; }
        public string? CenterName { get; set; }
        public DateTime? TermStartDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
