using System.Collections.Generic;
using System.Text;
using dtc.Domain.Entities.Training;

namespace dtc.Application.Features.AI.ChunkBuilders
{
    internal static class ResourceChunkBuilder
    {
        public static KnowledgeChunkPayload Build(ResourceLearning resource, string? courseName)
        {
            var text = new StringBuilder()
                .AppendLine("Loai tai lieu: Tai nguyen hoc tap.")
                .AppendLine($"Tieu de tai nguyen: {resource.Title}")
                .AppendLine($"Khoa hoc lien ket: {courseName ?? "Khong ro"}")
                .AppendLine($"Loai tai nguyen: {resource.ResourceType}")
                .AppendLine($"Duong dan tai nguyen: {resource.ResourceUrl}")
                .ToString();

            return new KnowledgeChunkPayload
            {
                Id = $"resource-{resource.Id}",
                Text = text,
                Metadata = new Dictionary<string, string>
                {
                    ["documentType"] = "resource",
                    ["referenceId"] = resource.Id.ToString(),
                    ["title"] = resource.Title,
                    ["courseId"] = resource.CourseId.ToString(),
                    ["courseName"] = courseName ?? string.Empty,
                    ["resourceType"] = resource.ResourceType.ToString()
                }
            };
        }
    }
}
