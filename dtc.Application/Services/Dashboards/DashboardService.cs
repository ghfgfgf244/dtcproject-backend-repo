using dtc.Application.DTOs.Dashboards;
using dtc.Application.Interfaces.Dashboards;
using dtc.Domain.Interfaces;
using dtc.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Dashboards
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FinanceDashboardResponseDto> GetFinanceDashboardAsync()
        {
            var courseRegs = await _unitOfWork.CourseRegistrations.GetAllAsync();
            var examRegs = await _unitOfWork.ExamRegistrations.GetAllAsync();
            var commissions = await _unitOfWork.CollaboratorCommissions.GetAllAsync();

            var approvedCourseRegs = courseRegs.Where(r => r.Status == CourseRegistrationStatus.Approved).ToList();
            var pendingCourseRegs = courseRegs.Where(r => r.Status == CourseRegistrationStatus.Pending).ToList();
            
            var approvedExamRegs = examRegs.Where(r => r.Status == ExamRegistrationStatus.Approved).ToList();
            var pendingExamRegs = examRegs.Where(r => r.Status == ExamRegistrationStatus.Pending).ToList();

            var totalRevenue = approvedCourseRegs.Sum(r => r.TotalFee); // Assuming ExamRegistration is free or bundled
            var totalDebt = pendingCourseRegs.Sum(r => r.TotalFee);
            var totalCommission = commissions.Sum(c => c.Amount); // Fixed property to Amount

            // Revenue Trend (Group by Year-Month of RegistrationDate)
            var trend = approvedCourseRegs
                .GroupBy(r => new { r.RegistrationDate.Year, r.RegistrationDate.Month })
                .Select(g => new MonthlyMetricDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Value = g.Sum(r => r.TotalFee)
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            return new FinanceDashboardResponseDto
            {
                TotalRevenue = totalRevenue,
                TotalDebt = totalDebt,
                TotalCommission = totalCommission,
                RevenueTrend = trend
            };
        }

        public async Task<AdmissionDashboardResponseDto> GetAdmissionDashboardAsync()
        {
            var courseRegs = await _unitOfWork.CourseRegistrations.GetAllAsync();
            var users = await _unitOfWork.Users.FindAsync(u => u.Roles.Any(r => r.RoleName == UserRole.Student), u => u.Roles);

            var pending = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Pending);
            var approved = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Approved);

            // Registration Trend (Count per month)
            var trend = courseRegs
                .GroupBy(r => new { r.RegistrationDate.Year, r.RegistrationDate.Month })
                .Select(g => new MonthlyMetricDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Value = g.Count()
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            return new AdmissionDashboardResponseDto
            {
                TotalStudents = users.Count(),
                TotalCourseRegistrations = courseRegs.Count(),
                PendingRegistrations = pending,
                ApprovedRegistrations = approved,
                RegistrationTrend = trend
            };
        }
    }
}
