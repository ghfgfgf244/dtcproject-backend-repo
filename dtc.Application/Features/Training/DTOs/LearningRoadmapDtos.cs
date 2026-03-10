using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class CreateLearningRoadmapRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int OrderNo { get; set; }
    }

    public class UpdateLearningRoadmapRequestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? OrderNo { get; set; }
    }

    public class LearningRoadmapResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OrderNo { get; set; }
    }
}
