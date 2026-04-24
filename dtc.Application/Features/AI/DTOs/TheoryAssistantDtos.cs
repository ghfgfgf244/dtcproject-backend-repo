using System.Collections.Generic;

namespace dtc.Application.Features.AI.DTOs
{
    public class TheoryAssistantRequestDto
    {
        public string Question { get; set; } = string.Empty;
        public string? ExamLevel { get; set; }
        public string? Category { get; set; }
        public bool IncludeStudyTips { get; set; } = true;
    }

    public class TheoryAssistantResponseDto
    {
        public string Answer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<AiSourceDto> Sources { get; set; } = [];
        public List<string> SuggestedTopics { get; set; } = [];
    }
}
