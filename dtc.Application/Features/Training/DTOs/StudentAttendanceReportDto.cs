using System;
using System.Collections.Generic;

namespace dtc.Application.Features.Training.DTOs
{
    public class StudentAttendanceReportDto
    {
        public StudentAttendanceSummaryDto Summary { get; set; } = new();
        public List<StudentAttendanceSessionDto> Sessions { get; set; } = new();
    }

    public class StudentAttendanceSummaryDto
    {
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class StudentAttendanceSessionDto
    {
        public Guid ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string LessonName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Present, Absent, Pending
    }
}
