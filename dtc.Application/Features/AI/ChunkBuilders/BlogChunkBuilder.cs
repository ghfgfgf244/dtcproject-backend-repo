using System.Collections.Generic;
using System.Text;
using dtc.Domain.Entities.Blogs;

namespace dtc.Application.Features.AI.ChunkBuilders
{
    internal static class BlogChunkBuilder
    {
        public static KnowledgeChunkPayload Build(Blog blog, string? categoryName)
        {
            var text = new StringBuilder()
                .AppendLine("Loai tai lieu: Bai viet huong dan hoc va thi GPLX.")
                .AppendLine($"Tieu de: {blog.Title}")
                .AppendLine($"Danh muc: {categoryName ?? $"Danh muc {blog.CategoryId}"}")
                .AppendLine($"Tom tat: {blog.Summary ?? "Khong co tom tat"}")
                .AppendLine($"Noi dung: {blog.Content}")
                .ToString();

            return new KnowledgeChunkPayload
            {
                Id = $"blog-{blog.Id}",
                Text = text,
                Metadata = new Dictionary<string, string>
                {
                    ["documentType"] = "blog",
                    ["referenceId"] = blog.Id.ToString(),
                    ["title"] = blog.Title,
                    ["category"] = categoryName ?? $"Danh muc {blog.CategoryId}",
                    ["status"] = blog.Status ? "published" : "draft"
                }
            };
        }
    }
}
