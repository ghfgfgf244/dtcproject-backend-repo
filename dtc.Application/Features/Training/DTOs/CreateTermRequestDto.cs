using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class CreateTermRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string TermName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
