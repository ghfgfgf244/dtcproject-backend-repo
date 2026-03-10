using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Training.Schedules
{
    public class UpdateClassScheduleRequestDto
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(255)]
        public string? Location { get; set; }
        
        public Guid? InstructorId { get; set; }
    }
}
