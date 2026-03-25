using System;

namespace dtc.Application.Features.Blogs.DTOs
{
    public class BlogResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Avatar { get; set; }
        public int CategoryId { get; set; }
        public string? Summary { get; set; }
        public string Content { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBlogRequestDto
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; }
        public string? Summary { get; set; }
        public string? Avatar { get; set; }
    }

    public class UpdateBlogRequestDto
    {
        public string? Title { get; set; }
        public int? CategoryId { get; set; }
        public string? Content { get; set; }
        public string? Summary { get; set; }
        public string? Avatar { get; set; }
    }
}
