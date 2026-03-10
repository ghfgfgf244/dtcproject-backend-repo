using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.DTOs.Training.Attendances
{
    public class MarkAttendanceRequestDto
    {
        [Required]
        public Guid ClassScheduleId { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public bool IsPresent { get; set; }
    }
}
