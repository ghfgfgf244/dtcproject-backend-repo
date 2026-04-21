using System.Collections.Generic;

namespace dtc.Application.Features.AI.DTOs
{
    public class ScheduleInsightRequestDto
    {
        public string Scenario { get; set; } = string.Empty;
        public string? Context { get; set; }
    }

    public class ScheduleInsightResponseDto
    {
        public string Summary { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<string> Suggestions { get; set; } = [];
    }
}
