using dtc.Application.Common;

namespace dtc.Application.Features.Training.DTOs
{
    public class TermPagedQueryDto : PagedRequest
    {
        public string? LicenseType { get; set; }
    }

    public class TermPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<TermResponseDto> Items { get; set; } = [];
    }
}
