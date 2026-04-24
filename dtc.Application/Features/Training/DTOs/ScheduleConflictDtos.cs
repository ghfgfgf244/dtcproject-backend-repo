using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dtc.Application.Features.Training.DTOs
{
    public class ScheduleConflictExplainRequestDto
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
        public int AddressId { get; set; }

        public Guid? IgnoreScheduleId { get; set; }
    }

    public class ScheduleConflictDetailDto
    {
        public string ConflictType { get; set; } = string.Empty;
        public Guid ScheduleId { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public string AddressName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ScheduleAlternativeSlotDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ScheduleConflictExplainResponseDto
    {
        public bool HasConflict { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<ScheduleConflictDetailDto> Conflicts { get; set; } = new();
        public List<ScheduleAlternativeSlotDto> Suggestions { get; set; } = new();
    }
}
