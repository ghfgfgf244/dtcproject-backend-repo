using System.Collections.Generic;

namespace dtc.Application.Features.AI.DTOs
{
    public class DashboardInsightRequestDto
    {
        public string Role { get; set; } = string.Empty;
        public string? ContextJson { get; set; }
    }

    public class DashboardInsightResponseDto
    {
        public string Summary { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<string> Highlights { get; set; } = [];
        public List<string> Alerts { get; set; } = [];
    }
}
