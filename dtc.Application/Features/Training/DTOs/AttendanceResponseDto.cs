using System;

namespace dtc.Application.Features.Training.DTOs
{
    public class AttendanceResponseDto
    {
        public Guid Id { get; set; }
        public Guid ClassScheduleId { get; set; }
        public Guid StudentId { get; set; }
        
        public string StudentName { get; set; } = string.Empty;
        public bool IsPresent { get; set; }
        public DateTime CheckedAt { get; set; }
    }
}
