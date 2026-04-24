using dtc.Application.Features.Dashboards.DTOs;
using dtc.Application.Features.Dashboards.Interfaces;
using dtc.Application.Features.AI.DTOs;
using dtc.Application.Features.AI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;
        private readonly IDashboardInsightService _dashboardInsightService;

        public DashboardController(
            IDashboardService dashboardService,
            IDashboardInsightService dashboardInsightService)
        {
            _dashboardService = dashboardService;
            _dashboardInsightService = dashboardInsightService;
        }

        // DEV-137: View finance dashboard
        [HttpGet("finance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFinanceDashboard()
        {
            var result = await _dashboardService.GetFinanceDashboardAsync();
            return Ok(result);
        }

        // DEV-138: View admission
        [HttpGet("admission")]
        [Authorize(Roles = "Admin,EnrollmentManager")]
        public async Task<IActionResult> GetAdmissionDashboard()
        {
            var result = await _dashboardService.GetAdmissionDashboardAsync(await GetManagedCenterIdAsync());
            return Ok(result);
        }

        [HttpGet("enrollment")]
        [Authorize(Roles = "Admin,EnrollmentManager")]
        public async Task<IActionResult> GetEnrollmentDashboard()
        {
            var result = await _dashboardService.GetEnrollmentDashboardAsync(await GetManagedCenterIdAsync());
            return Ok(result);
        }

        [HttpGet("training")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetTrainingDashboard()
        {
            var result = await _dashboardService.GetTrainingDashboardAsync(await GetManagedCenterIdAsync());
            return Ok(result);
        }

        [HttpGet("admin-overview")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _dashboardService.GetAdminDashboardAsync();
            return Ok(result);
        }

        [HttpGet("enrollment-ai-summary")]
        [Authorize(Roles = "Admin,EnrollmentManager")]
        public async Task<IActionResult> GetEnrollmentAiSummary()
        {
            var dashboard = await _dashboardService.GetEnrollmentDashboardAsync(await GetManagedCenterIdAsync());
            var summary = await _dashboardInsightService.SummarizeAsync(new DashboardInsightRequestDto
            {
                Role = "EnrollmentManager",
                ContextJson = JsonSerializer.Serialize(dashboard)
            });

            summary.Highlights =
            [
                $"Tỷ lệ duyệt hiện tại {dashboard.ApprovalRate:N1}% trên {dashboard.TotalRegistrations:N0} hồ sơ.",
                $"Có {dashboard.ActiveCollaborators:N0} cộng tác viên hoạt động trong kỳ gần đây.",
                dashboard.TopCourses.Count > 0
                    ? $"Khóa nổi bật nhất là {dashboard.TopCourses[0].CourseName} với {dashboard.TopCourses[0].TotalRegistrations:N0} lượt đăng ký."
                    : "Chưa có khóa học nổi bật trong giai đoạn này."
            ];
            summary.Alerts =
            [
                dashboard.PendingBacklogOlderThan3Days > 0
                    ? $"{dashboard.PendingBacklogOlderThan3Days:N0} hồ sơ chờ quá 3 ngày cần ưu tiên xử lý."
                    : "Không có hồ sơ tồn quá 3 ngày.",
                dashboard.PendingRegistrations > 0
                    ? $"{dashboard.PendingRegistrations:N0} hồ sơ vẫn đang chờ duyệt."
                    : "Không còn hồ sơ chờ duyệt."
            ];

            return Ok(summary);
        }

        [HttpGet("training-ai-summary")]
        [Authorize(Roles = "Admin,TrainingManager")]
        public async Task<IActionResult> GetTrainingAiSummary()
        {
            var dashboard = await _dashboardService.GetTrainingDashboardAsync(await GetManagedCenterIdAsync());
            var summary = await _dashboardInsightService.SummarizeAsync(new DashboardInsightRequestDto
            {
                Role = "TrainingManager",
                ContextJson = JsonSerializer.Serialize(dashboard)
            });

            summary.Highlights =
            [
                $"Hệ thống đang quản lý {dashboard.TotalClasses:N0} lớp với {dashboard.ActiveInstructors:N0} giảng viên hoạt động.",
                $"Tuần này có {dashboard.ClassesStartingThisWeek:N0} ca học bắt đầu theo lịch.",
                dashboard.UpcomingExamBatches.Count > 0
                    ? $"Đợt thi gần nhất là {dashboard.UpcomingExamBatches[0].BatchName} vào ngày {dashboard.UpcomingExamBatches[0].ExamDate:dd/MM/yyyy}."
                    : "Chưa có đợt thi sắp tới trong dữ liệu hiện tại."
            ];
            summary.Alerts =
            [
                dashboard.ScheduleConflictCount > 0
                    ? $"Phát hiện {dashboard.ScheduleConflictCount:N0} cặp lịch có nguy cơ xung đột."
                    : "Chưa phát hiện xung đột lịch học.",
                dashboard.LowAttendanceClasses.Count > 0
                    ? $"{dashboard.LowAttendanceClasses.Count:N0} lớp có tỷ lệ chuyên cần thấp cần theo dõi."
                    : "Chuyên cần đang ổn định ở các lớp đã điểm danh."
            ];

            return Ok(summary);
        }

        [HttpGet("admin-ai-summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminAiSummary()
        {
            var dashboard = await _dashboardService.GetAdminDashboardAsync();
            var summary = await _dashboardInsightService.SummarizeAsync(new DashboardInsightRequestDto
            {
                Role = "Admin",
                ContextJson = JsonSerializer.Serialize(dashboard)
            });

            summary.Highlights =
            [
                $"Tổng doanh thu ghi nhận hiện tại là {dashboard.TotalRevenue:N0} đ.",
                $"Toàn hệ thống có {dashboard.ActiveCenters:N0} trung tâm và {dashboard.ActiveCourses:N0} khóa học đang hoạt động.",
                dashboard.CenterPerformance.Count > 0
                    ? $"Trung tâm dẫn đầu hiện tại là {dashboard.CenterPerformance[0].CenterName}."
                    : "Chưa có dữ liệu hiệu suất trung tâm để xếp hạng."
            ];
            summary.Alerts =
            [
                dashboard.PendingExamBatches > 0
                    ? $"{dashboard.PendingExamBatches:N0} đợt thi đang chờ phê duyệt từ admin."
                    : "Không có đợt thi nào đang chờ phê duyệt.",
                dashboard.PendingCourseRegistrations > 0
                    ? $"{dashboard.PendingCourseRegistrations:N0} hồ sơ khóa học vẫn đang chờ xử lý."
                    : "Không có hồ sơ khóa học tồn đọng."
            ];

            return Ok(summary);
        }
    }
}
