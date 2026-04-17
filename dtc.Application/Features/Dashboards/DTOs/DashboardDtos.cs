using System;
using System.Collections.Generic;

namespace dtc.Application.Features.Dashboards.DTOs
{
    public class FinanceDashboardResponseDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalDebt { get; set; }
        public List<MonthlyMetricDto> RevenueTrend { get; set; } = new List<MonthlyMetricDto>();
    }

    public class AdmissionDashboardResponseDto
    {
        public int TotalStudents { get; set; }
        public int TotalCourseRegistrations { get; set; }
        public int PendingRegistrations { get; set; }
        public int ApprovedRegistrations { get; set; }
        public List<MonthlyMetricDto> RegistrationTrend { get; set; } = new List<MonthlyMetricDto>();
    }

    public class MonthlyMetricDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Value { get; set; } // Can be money or count
    }

    public class DashboardKpiDto
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string Tone { get; set; } = "default";
    }

    public class DashboardCourseMetricDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? LicenseType { get; set; }
        public int TotalRegistrations { get; set; }
        public int PendingRegistrations { get; set; }
        public int ApprovedRegistrations { get; set; }
    }

    public class DashboardCollaboratorMetricDto
    {
        public Guid CollaboratorId { get; set; }
        public string CollaboratorName { get; set; } = string.Empty;
        public string? ReferralCode { get; set; }
        public int ReferralRegistrations { get; set; }
        public decimal PendingCommission { get; set; }
        public decimal PaidCommission { get; set; }
    }

    public class DashboardRecentPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EnrollmentOperationalDashboardDto
    {
        public List<DashboardKpiDto> Kpis { get; set; } = new();
        public List<MonthlyMetricDto> RegistrationTrend { get; set; } = new();
        public List<DashboardCourseMetricDto> TopCourses { get; set; } = new();
        public List<DashboardCollaboratorMetricDto> TopCollaborators { get; set; } = new();
        public List<DashboardRecentPostDto> RecentPosts { get; set; } = new();
        public int TotalStudents { get; set; }
        public int TotalRegistrations { get; set; }
        public int PendingRegistrations { get; set; }
        public int ApprovedRegistrations { get; set; }
        public int RejectedRegistrations { get; set; }
        public int CancelledRegistrations { get; set; }
        public int ActiveCollaborators { get; set; }
        public int PendingBacklogOlderThan3Days { get; set; }
        public decimal ApprovalRate { get; set; }
    }

    public class DashboardUpcomingExamDto
    {
        public Guid Id { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int CurrentCandidates { get; set; }
        public int MaxCandidates { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DashboardInstructorLoadDto
    {
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int AssignedClasses { get; set; }
        public int SchedulesThisWeek { get; set; }
        public decimal UtilizationRate { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
    }

    public class DashboardAttendanceAlertDto
    {
        public Guid ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal AttendanceRate { get; set; }
        public int PresentCount { get; set; }
        public int TotalRecords { get; set; }
    }

    public class TrainingOperationalDashboardDto
    {
        public List<DashboardKpiDto> Kpis { get; set; } = new();
        public List<MonthlyMetricDto> ClassOpeningTrend { get; set; } = new();
        public List<DashboardUpcomingExamDto> UpcomingExamBatches { get; set; } = new();
        public List<DashboardInstructorLoadDto> InstructorLoads { get; set; } = new();
        public List<DashboardAttendanceAlertDto> LowAttendanceClasses { get; set; } = new();
        public int TotalClasses { get; set; }
        public int TheoryClasses { get; set; }
        public int PracticeClasses { get; set; }
        public int ActiveInstructors { get; set; }
        public int ScheduleConflictCount { get; set; }
        public int ClassesStartingThisWeek { get; set; }
    }

    public class DashboardCenterPerformanceDto
    {
        public Guid CenterId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public int TotalCourses { get; set; }
        public int ActiveTerms { get; set; }
        public int ActiveClasses { get; set; }
        public int TotalRegistrations { get; set; }
        public int ApprovedRegistrations { get; set; }
        public decimal Revenue { get; set; }
    }

    public class AdminOperationalDashboardDto
    {
        public List<DashboardKpiDto> Kpis { get; set; } = new();
        public List<MonthlyMetricDto> RevenueTrend { get; set; } = new();
        public List<DashboardCenterPerformanceDto> CenterPerformance { get; set; } = new();
        public List<DashboardUpcomingExamDto> PendingExamApprovals { get; set; } = new();
        public int TotalUsers { get; set; }
        public int ActiveCenters { get; set; }
        public int ActiveCourses { get; set; }
        public int PendingCourseRegistrations { get; set; }
        public int PendingExamBatches { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommission { get; set; }
    }
}
