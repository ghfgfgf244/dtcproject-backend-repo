using dtc.Domain.Entities;
using dtc.Domain.Entities.Training;
using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class CreateResourceLearningRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Url]
        public string ResourceUrl { get; set; } = string.Empty;
    }

    public class UpdateResourceLearningRequestDto
    {
        public ResourceType? ResourceType { get; set; }
        public string? Title { get; set; }
        
        [Url]
        public string? ResourceUrl { get; set; }
    }

    public class ResourceLearningResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public ResourceType ResourceType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResourceUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
