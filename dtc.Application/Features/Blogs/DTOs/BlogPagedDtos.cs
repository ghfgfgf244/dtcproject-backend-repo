using dtc.Application.Common;

namespace dtc.Application.Features.Blogs.DTOs
{
    public class BlogPagedQueryDto : PagedRequest
    {
        public bool OnlyPublished { get; set; }
        public string? SearchTerm { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class BlogPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<BlogResponseDto> Items { get; set; } = [];
    }
}
