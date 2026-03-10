using System.ComponentModel.DataAnnotations;
using dtc.Domain.Entities;

namespace dtc.Application.DTOs.Training.Registrations
{
    public class UpdateRegistrationStatusDto
    {
        [Required]
        public CourseRegistrationStatus Status { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
