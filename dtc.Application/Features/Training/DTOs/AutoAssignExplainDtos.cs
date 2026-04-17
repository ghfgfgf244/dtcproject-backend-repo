using System;
using System.Collections.Generic;

namespace dtc.Application.Features.Training.DTOs
{
    public class AutoAssignPreviewClassDto
    {
        public string ClassName { get; set; } = string.Empty;
        public string ClassType { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int SuggestedMaxStudents { get; set; }
        public double OccupancyRate { get; set; }
    }

    public class AutoAssignClassesExplainResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int EligibleStudents { get; set; }
        public int PlannedClassCount { get; set; }
        public int TargetClassSize { get; set; }
        public int MinSuggestedSize { get; set; }
        public int MaxSuggestedSize { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<string> Notes { get; set; } = new();
        public List<AutoAssignPreviewClassDto> Classes { get; set; } = new();
    }
}
