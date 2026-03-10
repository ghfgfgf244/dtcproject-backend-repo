using System;

namespace dtc.Application.DTOs.Training.Schedules
{
    public class ClassScheduleResponseDto
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Guid InstructorId { get; set; }
        
        public string InstructorName { get; set; } = string.Empty; // Useful for UI
        public string ClassName { get; set; } = string.Empty;     // Useful for UI
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
    }
}
