using dtc.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class CreateCourseRequestDto
    {
        [Required]
        public Guid CenterId { get; set; }

        [Required]
        [MaxLength(255)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public ExamLevel LicenseType { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DurationInWeeks { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxStudents { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string? ThumbnailUrl { get; set; }
    }
}
