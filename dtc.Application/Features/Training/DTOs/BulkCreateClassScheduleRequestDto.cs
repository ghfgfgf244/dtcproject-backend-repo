using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class BulkCreateClassScheduleRequestDto
    {
        [Required]
        public Guid ClassId { get; set; }

        [Required]
        [MinLength(1)]
        public List<ClassScheduleDraftDto> Schedules { get; set; } = new();
    }
}
