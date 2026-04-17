using System.Collections.Generic;

namespace dtc.Application.Features.AI.DTOs
{
    public class AiSourceDto
    {
        public string Title { get; set; } = string.Empty;
        public string Snippet { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string? ReferenceId { get; set; }
    }

    public class AiChatRequestDto
    {
        public string Prompt { get; set; } = string.Empty;
        public string? Scope { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class AiChatResponseDto
    {
        public string Answer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public bool FromCache { get; set; }
        public List<AiSourceDto> Sources { get; set; } = [];
    }
}
