using System.Collections.Generic;

namespace dtc.Application.Features.Training.DTOs
{
    public class AutoAssignClassesResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int EligibleStudents { get; set; }
        public int CreatedClasses { get; set; }
        public List<ClassResponseDto> Classes { get; set; } = new();
    }
}
