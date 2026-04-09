using System.Collections.Generic;

namespace dtc.Application.Features.Training.DTOs
{
    public class ScheduleImportPreviewDto
    {
        public List<ClassScheduleDraftDto> Schedules { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
