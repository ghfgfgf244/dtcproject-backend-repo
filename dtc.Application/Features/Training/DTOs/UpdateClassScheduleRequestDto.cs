using System;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class UpdateClassScheduleRequestDto
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public int? AddressId { get; set; }
        
        public Guid? InstructorId { get; set; }
    }
}
