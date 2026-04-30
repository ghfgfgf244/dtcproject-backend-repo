using dtc.Application.Common;

namespace dtc.Application.Features.Training.DTOs
{
    public class CourseRegistrationPagedQueryDto : PagedRequest
    {
        public string? Search { get; set; }
        public string? LicenseType { get; set; }
        public string? Status { get; set; }
    }

    public class CourseRegistrationPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int NewRegistrationsThisMonth { get; set; }
        public int PendingRegistrations { get; set; }
        public IEnumerable<CourseRegistrationResponseDto> Items { get; set; } = [];
    }
}
