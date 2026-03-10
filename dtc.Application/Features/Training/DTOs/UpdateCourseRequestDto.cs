using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class UpdateCourseRequestDto
    {
        [MaxLength(255)]
        public string? CourseName { get; set; }
        
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxStudents { get; set; }

        [Range(1, int.MaxValue)]
        public int? DurationInWeeks { get; set; }
    }
}
