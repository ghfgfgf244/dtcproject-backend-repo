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

        public string? ReferralCode { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? Photo { get; set; }
        public Microsoft.AspNetCore.Http.IFormFile? IdFront { get; set; }
        public Microsoft.AspNetCore.Http.IFormFile? IdBack { get; set; }
    }
}
