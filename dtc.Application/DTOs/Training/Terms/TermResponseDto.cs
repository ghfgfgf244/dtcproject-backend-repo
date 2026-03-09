using System;

namespace dtc.Application.DTOs.Training.Terms
{
    public class TermResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string TermName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
