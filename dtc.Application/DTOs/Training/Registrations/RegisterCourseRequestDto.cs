using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Training.Registrations
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
