using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class RegisterCourseRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalFee { get; set; }

        public string? Notes { get; set; }
    }
}
