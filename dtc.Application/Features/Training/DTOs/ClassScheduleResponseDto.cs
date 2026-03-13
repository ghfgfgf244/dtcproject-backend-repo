using System;

namespace dtc.Application.Features.Training.DTOs
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
