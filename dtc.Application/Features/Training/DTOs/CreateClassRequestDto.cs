using System;
using System.ComponentModel.DataAnnotations;
using dtc.Domain.Entities;

namespace dtc.Application.Features.Training.DTOs
{
    public class CreateClassRequestDto
    {
        [Required]
        public Guid TermId { get; set; }

        [Required]
        public Guid InstructorId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxStudents { get; set; }

        [Required]
        public ClassType ClassType { get; set; }
    }
}
