using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Training.Schedules
{
    public class CreateClassScheduleRequestDto
    {
        [Required]
        public Guid ClassId { get; set; }

        [Required]
        public Guid InstructorId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;
    }
}
