using dtc.Domain.Entities;

namespace dtc.Application.Features.Training.DTOs
{
    public class CourseResponseDto
    {
        public Guid Id { get; set; }
        public Guid CenterId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty; // Converted to string
        public int DurationInWeeks { get; set; }
        public int MaxStudents { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
