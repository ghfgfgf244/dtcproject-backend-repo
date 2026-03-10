using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class CreateClassRequestDto
    {
        [Required]
        public Guid TermId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxStudents { get; set; }
    }
}
