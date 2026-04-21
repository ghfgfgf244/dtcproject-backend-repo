using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dtc.Application.Features.Dashboards.DTOs;
using dtc.Application.Features.Dashboards.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Collaborators;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Location;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.Dashboards.Services
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
            var courseRegs = (await _unitOfWork.CourseRegistrations.GetAllAsync()).ToList();
            var examRegs = (await _unitOfWork.ExamRegistrations.GetAllAsync()).ToList();
            var commissions = (await _unitOfWork.CollaboratorCommissions.GetAllAsync()).ToList();

            var approvedCourseRegs = courseRegs.Where(r => r.Status == CourseRegistrationStatus.Approved).ToList();
            var pendingCourseRegs = courseRegs.Where(r => r.Status == CourseRegistrationStatus.Pending).ToList();

            var approvedExamRegs = examRegs.Where(r => r.Status == ExamRegistrationStatus.Approved).ToList();
            var pendingExamRegs = examRegs.Where(r => r.Status == ExamRegistrationStatus.Pending).ToList();

            var totalRevenue = approvedCourseRegs.Sum(r => r.TotalFee);
            var totalDebt = pendingCourseRegs.Sum(r => r.TotalFee);
            var totalCommission = commissions.Sum(c => c.Amount);

            var trend = approvedCourseRegs
                .GroupBy(r => new { r.RegistrationDate.Year, r.RegistrationDate.Month })
                .Select(g => new MonthlyMetricDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Value = g.Sum(r => r.TotalFee)
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
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
            var courseRegs = (await _unitOfWork.CourseRegistrations.GetAllAsync()).ToList();
            var users = (await _unitOfWork.Users.FindAsync(u => u.RoleId == UserRole.Student)).ToList();

            var pending = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Pending);
            var approved = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Approved);

            var trend = courseRegs
                .GroupBy(r => new { r.RegistrationDate.Year, r.RegistrationDate.Month })
                .Select(g => new MonthlyMetricDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Value = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return new AdmissionDashboardResponseDto
            {
                TotalStudents = users.Count,
                TotalCourseRegistrations = courseRegs.Count,
                PendingRegistrations = pending,
                ApprovedRegistrations = approved,
                RegistrationTrend = trend
            };
        }

        public async Task<EnrollmentOperationalDashboardDto> GetEnrollmentDashboardAsync()
        {
            var users = (await _unitOfWork.Users.GetAllAsync()).ToList();
            var courses = (await _unitOfWork.Courses.GetAllAsync()).ToList();
            var courseRegs = (await _unitOfWork.CourseRegistrations.GetAllAsync()).ToList();
            var referralCodes = (await _unitOfWork.ReferralCodes.GetAllAsync()).ToList();
            var referralRegs = (await _unitOfWork.ReferralRegistrations.GetAllAsync()).ToList();
            var commissions = (await _unitOfWork.CollaboratorCommissions.GetAllAsync()).ToList();
            var blogs = (await _unitOfWork.Blogs.GetAllAsync()).ToList();
            var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();

            var students = users.Where(u => u.RoleId == UserRole.Student && u.IsActive).ToList();
            var collaborators = users.Where(u => u.RoleId == UserRole.Collaborator && u.IsActive).ToList();

            var pending = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Pending);
            var approved = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Approved);
            var rejected = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Rejected);
            var cancelled = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Cancelled);
            var backlogOlderThan3Days = courseRegs.Count(r =>
                r.Status == CourseRegistrationStatus.Pending &&
                r.RegistrationDate <= DateTime.UtcNow.AddDays(-3));

            var totalRegistrations = courseRegs.Count;
            var approvalRate = totalRegistrations == 0
                ? 0
                : Math.Round((decimal)approved / totalRegistrations * 100m, 1);

            var courseLookup = courses.ToDictionary(c => c.Id, c => c);
            var categoryLookup = categories
                .GroupBy(c => c.CategoryId)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(item => item.CategoryName)
                        .LastOrDefault(name => !string.IsNullOrWhiteSpace(name)) ?? string.Empty);

            var topCourses = courseRegs
                .GroupBy(r => r.CourseId)
                .Select(group =>
                {
                    courseLookup.TryGetValue(group.Key, out var course);
                    return new DashboardCourseMetricDto
                    {
                        CourseId = group.Key,
                        CourseName = course?.CourseName ?? "Khóa học đã xóa",
                        LicenseType = course?.LicenseType.ToString(),
                        TotalRegistrations = group.Count(),
                        PendingRegistrations = group.Count(x => x.Status == CourseRegistrationStatus.Pending),
                        ApprovedRegistrations = group.Count(x => x.Status == CourseRegistrationStatus.Approved)
                    };
                })
                .OrderByDescending(x => x.TotalRegistrations)
                .ThenBy(x => x.CourseName)
                .Take(5)
                .ToList();

            var topCollaborators = referralRegs
                .GroupBy(r => referralCodes.FirstOrDefault(code => code.Id == r.ReferralCodeId)?.CollaboratorId ?? Guid.Empty)
                .Where(group => group.Key != Guid.Empty)
                .Select(group =>
                {
                    var collaborator = collaborators.FirstOrDefault(u => u.Id == group.Key);
                    var code = referralCodes.FirstOrDefault(rc => rc.CollaboratorId == group.Key);
                    var collaboratorCommissions = commissions.Where(c => c.CollaboratorId == group.Key).ToList();

                    return new DashboardCollaboratorMetricDto
                    {
                        CollaboratorId = group.Key,
                        CollaboratorName = collaborator?.FullName ?? "Cộng tác viên",
                        ReferralCode = code?.Code,
                        ReferralRegistrations = group.Count(),
                        PendingCommission = collaboratorCommissions
                            .Where(c => c.Status == CommissionStatus.Pending)
                            .Sum(c => c.Amount),
                        PaidCommission = collaboratorCommissions
                            .Where(c => c.Status == CommissionStatus.Paid)
                            .Sum(c => c.Amount)
                    };
                })
                .OrderByDescending(x => x.ReferralRegistrations)
                .ThenBy(x => x.CollaboratorName)
                .Take(5)
                .ToList();

            var recentPosts = blogs
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(blog => new DashboardRecentPostDto
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    CategoryName = categoryLookup.TryGetValue(blog.CategoryId, out var categoryName) ? categoryName : null,
                    IsPublished = blog.Status,
                    CreatedAt = blog.CreatedAt
                })
                .ToList();

            return new EnrollmentOperationalDashboardDto
            {
                TotalStudents = students.Count,
                TotalRegistrations = totalRegistrations,
                PendingRegistrations = pending,
                ApprovedRegistrations = approved,
                RejectedRegistrations = rejected,
                CancelledRegistrations = cancelled,
                ActiveCollaborators = collaborators.Count,
                PendingBacklogOlderThan3Days = backlogOlderThan3Days,
                ApprovalRate = approvalRate,
                Kpis = new List<DashboardKpiDto>
                {
                    new() { Title = "Học viên đang hoạt động", Value = students.Count.ToString("N0"), Note = "Tài khoản role Student đang bật", Tone = "primary" },
                    new() { Title = "Hồ sơ chờ duyệt", Value = pending.ToString("N0"), Note = $"{backlogOlderThan3Days:N0} hồ sơ tồn quá 3 ngày", Tone = backlogOlderThan3Days > 0 ? "warning" : "neutral" },
                    new() { Title = "Tỷ lệ duyệt", Value = $"{approvalRate:N1}%", Note = $"{approved:N0}/{totalRegistrations:N0} đăng ký được duyệt", Tone = "success" },
                    new() { Title = "Cộng tác viên hoạt động", Value = collaborators.Count.ToString("N0"), Note = $"{topCollaborators.Sum(x => x.ReferralRegistrations):N0} lượt giới thiệu", Tone = "info" }
                },
                RegistrationTrend = BuildRecentMonthlyMetrics(
                    courseRegs,
                    registration => registration.RegistrationDate,
                    registrations => registrations.Count()),
                TopCourses = topCourses,
                TopCollaborators = topCollaborators,
                RecentPosts = recentPosts
            };
        }

        public async Task<TrainingOperationalDashboardDto> GetTrainingDashboardAsync()
        {
            var classes = (await _unitOfWork.Classes.GetAllAsync()).ToList();
            var schedules = (await _unitOfWork.ClassSchedules.GetAllAsync()).ToList();
            var attendances = (await _unitOfWork.Attendances.GetAllAsync()).ToList();
            var instructors = (await _unitOfWork.Users.FindAsync(u => u.RoleId == UserRole.Instructor && u.IsActive)).ToList();
            var examBatches = (await _unitOfWork.ExamBatches.GetAllAsync()).ToList();

            var activeClasses = classes.Where(c => c.Status != ClassStatus.Cancelled).ToList();
            var theoryClasses = activeClasses.Count(c => c.ClassType == ClassType.Theory);
            var practiceClasses = activeClasses.Count(c => c.ClassType == ClassType.Practice);
            var now = DateTime.UtcNow;
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
            if (startOfWeek > now.Date)
            {
                startOfWeek = startOfWeek.AddDays(-7);
            }

            var endOfWeek = startOfWeek.AddDays(7);
            var classesStartingThisWeek = schedules
                .Count(s => s.StartTime >= startOfWeek && s.StartTime < endOfWeek);

            var instructorLoads = instructors
                .Select(instructor =>
                {
                    var instructorClassIds = activeClasses
                        .Where(c => c.InstructorId == instructor.Id)
                        .Select(c => c.Id)
                        .ToHashSet();

                    var instructorSchedules = schedules
                        .Where(s => s.InstructorId == instructor.Id)
                        .ToList();

                    var weekSchedules = instructorSchedules
                        .Count(s => s.StartTime >= startOfWeek && s.StartTime < endOfWeek);

                    var isTeachingNow = instructorSchedules.Any(s => s.StartTime <= now && s.EndTime >= now);
                    var utilization = Math.Min(100m, Math.Round((decimal)weekSchedules / 10m * 100m, 1));

                    return new DashboardInstructorLoadDto
                    {
                        InstructorId = instructor.Id,
                        InstructorName = instructor.FullName,
                        AssignedClasses = instructorClassIds.Count,
                        SchedulesThisWeek = weekSchedules,
                        UtilizationRate = utilization,
                        StatusLabel = isTeachingNow ? "Đang dạy" : instructorClassIds.Count == 0 ? "Rảnh rỗi" : "Sẵn sàng"
                    };
                })
                .OrderByDescending(x => x.SchedulesThisWeek)
                .ThenBy(x => x.InstructorName)
                .Take(8)
                .ToList();

            var lowAttendanceClasses = attendances
                .Join(schedules, attendance => attendance.ClassScheduleId, schedule => schedule.Id, (attendance, schedule) => new { attendance, schedule })
                .GroupBy(x => x.schedule.ClassId)
                .Select(group =>
                {
                    var classInfo = classes.FirstOrDefault(c => c.Id == group.Key);
                    var totalRecords = group.Count();
                    var presentCount = group.Count(x => x.attendance.IsPresent);
                    var attendanceRate = totalRecords == 0
                        ? 0
                        : Math.Round((decimal)presentCount / totalRecords * 100m, 1);

                    return new DashboardAttendanceAlertDto
                    {
                        ClassId = group.Key,
                        ClassName = classInfo?.ClassName ?? "Lớp học",
                        AttendanceRate = attendanceRate,
                        PresentCount = presentCount,
                        TotalRecords = totalRecords
                    };
                })
                .Where(x => x.TotalRecords > 0)
                .OrderBy(x => x.AttendanceRate)
                .ThenByDescending(x => x.TotalRecords)
                .Take(5)
                .ToList();

            var upcomingExamBatches = examBatches
                .Where(batch => batch.ExamStartDate >= now)
                .OrderBy(batch => batch.ExamStartDate)
                .Take(5)
                .Select(batch => new DashboardUpcomingExamDto
                {
                    Id = batch.Id,
                    BatchName = batch.BatchName,
                    ExamDate = batch.ExamStartDate,
                    CurrentCandidates = batch.CurrentCandidates,
                    MaxCandidates = batch.MaxCandidates,
                    Status = batch.Status.ToString()
                })
                .ToList();

            var scheduleConflictCount = CountScheduleConflicts(schedules);

            return new TrainingOperationalDashboardDto
            {
                TotalClasses = activeClasses.Count,
                TheoryClasses = theoryClasses,
                PracticeClasses = practiceClasses,
                ActiveInstructors = instructors.Count,
                ScheduleConflictCount = scheduleConflictCount,
                ClassesStartingThisWeek = classesStartingThisWeek,
                Kpis = new List<DashboardKpiDto>
                {
                    new() { Title = "Lớp đang quản lý", Value = activeClasses.Count.ToString("N0"), Note = $"{theoryClasses:N0} lý thuyết, {practiceClasses:N0} thực hành", Tone = "primary" },
                    new() { Title = "Giảng viên hoạt động", Value = instructors.Count.ToString("N0"), Note = $"{instructorLoads.Count(x => x.StatusLabel == "Đang dạy"):N0} người đang lên lớp", Tone = "info" },
                    new() { Title = "Ca học tuần này", Value = classesStartingThisWeek.ToString("N0"), Note = $"{schedules.Count(s => s.StartTime >= now):N0} lịch còn lại trong tương lai", Tone = "success" },
                    new() { Title = "Cảnh báo xung đột", Value = scheduleConflictCount.ToString("N0"), Note = $"{lowAttendanceClasses.Count:N0} lớp có chuyên cần thấp", Tone = scheduleConflictCount > 0 ? "warning" : "neutral" }
                },
                ClassOpeningTrend = BuildRecentMonthlyMetrics(
                    classes,
                    @class => @class.CreatedAt,
                    data => data.Count()),
                UpcomingExamBatches = upcomingExamBatches,
                InstructorLoads = instructorLoads,
                LowAttendanceClasses = lowAttendanceClasses
            };
        }

        public async Task<AdminOperationalDashboardDto> GetAdminDashboardAsync()
        {
            var users = (await _unitOfWork.Users.GetAllAsync()).ToList();
            var centers = (await _unitOfWork.Centers.GetAllAsync()).ToList();
            var courses = (await _unitOfWork.Courses.GetAllAsync()).ToList();
            var terms = (await _unitOfWork.Terms.GetAllAsync()).ToList();
            var classes = (await _unitOfWork.Classes.GetAllAsync()).ToList();
            var courseRegs = (await _unitOfWork.CourseRegistrations.GetAllAsync()).ToList();
            var examBatches = (await _unitOfWork.ExamBatches.GetAllAsync()).ToList();
            var commissions = (await _unitOfWork.CollaboratorCommissions.GetAllAsync()).ToList();

            var approvedCourseRegs = courseRegs.Where(r => r.Status == CourseRegistrationStatus.Approved).ToList();
            var totalRevenue = approvedCourseRegs.Sum(r => r.TotalFee);
            var totalCommission = commissions.Sum(c => c.Amount);
            var pendingCourseRegistrations = courseRegs.Count(r => r.Status == CourseRegistrationStatus.Pending);
            var pendingExamBatches = examBatches.Count(b => b.Status == ExamBatchStatus.Pending);

            var coursesByCenter = courses.GroupBy(c => c.CenterId).ToDictionary(g => g.Key, g => g.ToList());
            var termsByCourse = terms.GroupBy(t => t.CourseId).ToDictionary(g => g.Key, g => g.ToList());
            var classesByTerm = classes.GroupBy(c => c.TermId).ToDictionary(g => g.Key, g => g.ToList());

            var centerPerformance = centers
                .Select(center =>
                {
                    var centerCourses = coursesByCenter.TryGetValue(center.Id, out var centerCoursesValue)
                        ? centerCoursesValue
                        : new List<Course>();

                    var centerCourseIds = centerCourses.Select(c => c.Id).ToHashSet();
                    var centerTerms = terms.Where(t => centerCourseIds.Contains(t.CourseId)).ToList();
                    var centerTermIds = centerTerms.Select(t => t.Id).ToHashSet();
                    var centerClasses = classes.Where(c => centerTermIds.Contains(c.TermId) && c.Status != ClassStatus.Cancelled).ToList();
                    var centerRegistrations = courseRegs.Where(r => centerCourseIds.Contains(r.CourseId)).ToList();
                    var centerApprovedRegistrations = centerRegistrations.Where(r => r.Status == CourseRegistrationStatus.Approved).ToList();

                    return new DashboardCenterPerformanceDto
                    {
                        CenterId = center.Id,
                        CenterName = center.CenterName,
                        TotalCourses = centerCourses.Count,
                        ActiveTerms = centerTerms.Count(t => t.IsActive),
                        ActiveClasses = centerClasses.Count,
                        TotalRegistrations = centerRegistrations.Count,
                        ApprovedRegistrations = centerApprovedRegistrations.Count,
                        Revenue = centerApprovedRegistrations.Sum(r => r.TotalFee)
                    };
                })
                .OrderByDescending(x => x.Revenue)
                .ThenByDescending(x => x.TotalRegistrations)
                .Take(6)
                .ToList();

            var pendingExamApprovals = examBatches
                .Where(batch => batch.Status == ExamBatchStatus.Pending || batch.Status == ExamBatchStatus.OpenForRegistration)
                .OrderBy(batch => batch.ExamStartDate)
                .Take(6)
                .Select(batch => new DashboardUpcomingExamDto
                {
                    Id = batch.Id,
                    BatchName = batch.BatchName,
                    ExamDate = batch.ExamStartDate,
                    CurrentCandidates = batch.CurrentCandidates,
                    MaxCandidates = batch.MaxCandidates,
                    Status = batch.Status.ToString()
                })
                .ToList();

            return new AdminOperationalDashboardDto
            {
                TotalUsers = users.Count,
                ActiveCenters = centers.Count(c => c.IsActive),
                ActiveCourses = courses.Count(c => c.IsActive),
                PendingCourseRegistrations = pendingCourseRegistrations,
                PendingExamBatches = pendingExamBatches,
                TotalRevenue = totalRevenue,
                TotalCommission = totalCommission,
                Kpis = new List<DashboardKpiDto>
                {
                    new() { Title = "Người dùng toàn hệ thống", Value = users.Count.ToString("N0"), Note = $"{users.Count(u => u.IsActive):N0} tài khoản đang hoạt động", Tone = "primary" },
                    new() { Title = "Trung tâm hoạt động", Value = centers.Count(c => c.IsActive).ToString("N0"), Note = $"{courses.Count(c => c.IsActive):N0} khóa học đang mở", Tone = "info" },
                    new() { Title = "Doanh thu đã ghi nhận", Value = $"{totalRevenue:N0} đ", Note = $"{approvedCourseRegs.Count:N0} hồ sơ đã duyệt", Tone = "success" },
                    new() { Title = "Đợt thi chờ xử lý", Value = pendingExamBatches.ToString("N0"), Note = $"{pendingCourseRegistrations:N0} hồ sơ khóa học còn chờ", Tone = pendingExamBatches > 0 ? "warning" : "neutral" }
                },
                RevenueTrend = BuildRecentMonthlyMetrics(
                    approvedCourseRegs,
                    registration => registration.RegistrationDate,
                    registrations => registrations.Sum(x => x.TotalFee)),
                CenterPerformance = centerPerformance,
                PendingExamApprovals = pendingExamApprovals
            };
        }

        private static List<MonthlyMetricDto> BuildRecentMonthlyMetrics<T>(
            IEnumerable<T> source,
            Func<T, DateTime> dateSelector,
            Func<IEnumerable<T>, decimal> valueSelector,
            int monthCount = 6)
        {
            var utcNow = DateTime.UtcNow;
            var months = Enumerable.Range(0, monthCount)
                .Select(offset =>
                {
                    var month = utcNow.AddMonths(-(monthCount - 1 - offset));
                    return new { month.Year, month.Month };
                })
                .ToList();

            return months
                .Select(month =>
                {
                    var items = source.Where(item =>
                    {
                        var date = dateSelector(item);
                        return date.Year == month.Year && date.Month == month.Month;
                    });

                    return new MonthlyMetricDto
                    {
                        Year = month.Year,
                        Month = month.Month,
                        Value = valueSelector(items)
                    };
                })
                .ToList();
        }

        private static int CountScheduleConflicts(IReadOnlyList<ClassSchedule> schedules)
        {
            var conflictPairs = new HashSet<string>(StringComparer.Ordinal);

            void AddConflict(Guid leftId, Guid rightId)
            {
                var a = leftId.CompareTo(rightId) <= 0 ? leftId : rightId;
                var b = leftId.CompareTo(rightId) <= 0 ? rightId : leftId;
                conflictPairs.Add($"{a}:{b}");
            }

            foreach (var group in schedules.GroupBy(s => s.InstructorId))
            {
                var ordered = group.OrderBy(s => s.StartTime).ToList();
                for (var i = 0; i < ordered.Count; i++)
                {
                    for (var j = i + 1; j < ordered.Count; j++)
                    {
                        if (ordered[j].StartTime >= ordered[i].EndTime)
                        {
                            break;
                        }

                        if (ordered[i].ClassId != ordered[j].ClassId)
                        {
                            AddConflict(ordered[i].Id, ordered[j].Id);
                        }
                    }
                }
            }

            foreach (var group in schedules.GroupBy(s => s.AddressId))
            {
                var ordered = group.OrderBy(s => s.StartTime).ToList();
                for (var i = 0; i < ordered.Count; i++)
                {
                    for (var j = i + 1; j < ordered.Count; j++)
                    {
                        if (ordered[j].StartTime >= ordered[i].EndTime)
                        {
                            break;
                        }

                        if (ordered[i].ClassId != ordered[j].ClassId)
                        {
                            AddConflict(ordered[i].Id, ordered[j].Id);
                        }
                    }
                }
            }

            return conflictPairs.Count;
        }
    }
}
