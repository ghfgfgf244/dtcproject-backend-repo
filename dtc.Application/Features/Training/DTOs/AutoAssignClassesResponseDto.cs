using System.Collections.Generic;

namespace dtc.Application.Features.Training.DTOs
{
    public class AutoAssignClassesResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int EligibleStudents { get; set; }
        public int CreatedClasses { get; set; }
        public int TargetClassSize { get; set; }
        public int MinSuggestedSize { get; set; }
        public int MaxSuggestedSize { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<string> Notes { get; set; } = new();
        public List<ClassResponseDto> Classes { get; set; } = new();
    }
}
