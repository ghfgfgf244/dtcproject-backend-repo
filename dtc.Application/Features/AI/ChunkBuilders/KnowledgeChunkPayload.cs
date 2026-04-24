using System.Collections.Generic;

namespace dtc.Application.Features.AI.ChunkBuilders
{
    public class KnowledgeChunkPayload
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = [];
    }
}
