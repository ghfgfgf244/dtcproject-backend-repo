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
}
