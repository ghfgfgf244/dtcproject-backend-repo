using System;
using System.Collections.Generic;

namespace dtc.Application.Features.AI.DTOs
{
    public class CourseAdvisorRequestDto
    {
        public string? DesiredLicenseLevel { get; set; }
        public string? PreferredDistrict { get; set; }
        public string? PreferredSchedule { get; set; }
        public bool NeedNearestCenter { get; set; }
        public bool NeedEarliestExam { get; set; }
    }

    public class CourseAdvisorSuggestionDto
    {
        public Guid? CourseId { get; set; }
        public Guid? CenterId { get; set; }
        public Guid? TermId { get; set; }
        public Guid? ExamBatchId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public string CenterAddress { get; set; } = string.Empty;
        public string? TermName { get; set; }
        public DateTime? TermStartDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public int? RemainingTermSeats { get; set; }
        public string? ExamBatchName { get; set; }
        public DateTime? ExamDate { get; set; }
        public string? ExamAddressName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class CourseAdvisorResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<CourseAdvisorSuggestionDto> Suggestions { get; set; } = [];
    }
}
