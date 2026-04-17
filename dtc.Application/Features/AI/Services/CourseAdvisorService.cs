using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;
using dtc.Application.Features.AI.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Location;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.AI.Services
{
    public class CourseAdvisorService : ICourseAdvisorService
    {
        private readonly IAiRouterService _aiRouterService;
        private readonly IUnitOfWork _unitOfWork;

        public CourseAdvisorService(IAiRouterService aiRouterService, IUnitOfWork unitOfWork)
        {
            _aiRouterService = aiRouterService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseAdvisorResponseDto> AdviseAsync(
            CourseAdvisorRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow.Date;
            var desiredLicense = NormalizeText(request.DesiredLicenseLevel);
            var preferredDistrict = NormalizeText(request.PreferredDistrict);

            var allCourses = (await _unitOfWork.Courses.FindAsync(c => c.IsActive && !c.IsDeleted, c => c.Center)).ToList();
            var filteredCourses = allCourses
                .Where(c => string.IsNullOrWhiteSpace(desiredLicense) || NormalizeText(c.LicenseType.ToString()) == desiredLicense)
                .ToList();

            var coursePool = filteredCourses.Count > 0 ? filteredCourses : allCourses;
            if (coursePool.Count == 0)
            {
                return new CourseAdvisorResponseDto
                {
                    Message = "Hiện tại hệ thống chưa có khóa học hoạt động để gợi ý.",
                    Model = "rule-based"
                };
            }

            var courseIds = coursePool.Select(c => c.Id).ToList();
            var terms = (await _unitOfWork.Terms.FindAsync(
                t => courseIds.Contains(t.CourseId) && !t.IsDeleted && t.IsActive && t.EndDate >= now))
                .ToList();

            var exams = (await _unitOfWork.Exams.FindAsync(
                e => courseIds.Contains(e.CourseId) && !e.IsDeleted && e.Status == ExamStatus.Scheduled && e.ExamDate >= now))
                .ToList();

            var batchIds = exams.Select(e => e.ExamBatchId).Distinct().ToList();
            var batches = batchIds.Count == 0
                ? new List<ExamBatch>()
                : (await _unitOfWork.ExamBatches.FindAsync(b => batchIds.Contains(b.Id) && !b.IsDeleted)).ToList();

            var addressIds = exams.Select(e => e.AddressId).Distinct().ToList();
            var addresses = addressIds.Count == 0
                ? new List<Address>()
                : (await _unitOfWork.Addresses.FindAsync(a => addressIds.Contains(a.Id))).ToList();

            var batchMap = batches.ToDictionary(b => b.Id);
            var addressMap = addresses.ToDictionary(a => a.Id);

            var rankedSuggestions = coursePool
                .Select(course =>
                {
                    var center = course.Center;
                    var nearestTerm = terms
                        .Where(t => t.CourseId == course.Id && t.CurrentStudents < t.MaxStudents)
                        .OrderBy(t => t.StartDate)
                        .FirstOrDefault();

                    var examCandidates = exams
                        .Where(e => e.CourseId == course.Id)
                        .OrderBy(e => e.ExamDate)
                        .ToList();

                    var nearestExam = examCandidates.FirstOrDefault();
                    var nearestBatch = nearestExam != null && batchMap.TryGetValue(nearestExam.ExamBatchId, out var batch)
                        ? batch
                        : null;
                    var nearestAddress = nearestExam != null && addressMap.TryGetValue(nearestExam.AddressId, out var address)
                        ? address
                        : null;

                    var score = CalculateScore(
                        course,
                        center?.Address,
                        nearestTerm,
                        nearestBatch,
                        desiredLicense,
                        preferredDistrict,
                        request.NeedNearestCenter,
                        request.NeedEarliestExam);

                    var summary = BuildSummary(course, center?.CenterName, center?.Address, nearestTerm, nearestBatch, nearestExam, nearestAddress);
                    var reason = BuildReason(course, center?.Address, nearestTerm, nearestBatch, preferredDistrict);

                    return new
                    {
                        Score = score,
                        Suggestion = new CourseAdvisorSuggestionDto
                        {
                            CourseId = course.Id,
                            CenterId = course.CenterId,
                            TermId = nearestTerm?.Id,
                            ExamBatchId = nearestBatch?.Id,
                            CourseName = course.CourseName,
                            LicenseType = course.LicenseType.ToString(),
                            Price = course.Price,
                            CenterName = center?.CenterName ?? "Trung tâm chưa xác định",
                            CenterAddress = center?.Address ?? string.Empty,
                            TermName = nearestTerm?.TermName,
                            TermStartDate = nearestTerm?.StartDate,
                            TermEndDate = nearestTerm?.EndDate,
                            RemainingTermSeats = nearestTerm != null ? nearestTerm.MaxStudents - nearestTerm.CurrentStudents : null,
                            ExamBatchName = nearestBatch?.BatchName,
                            ExamDate = nearestExam?.ExamDate,
                            ExamAddressName = nearestAddress?.AddressName,
                            Title = $"{course.CourseName} - {center?.CenterName ?? "Trung tâm"}",
                            Summary = summary,
                            Reason = reason
                        }
                    };
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Suggestion.TermStartDate ?? DateTime.MaxValue)
                .Take(3)
                .Select(x => x.Suggestion)
                .ToList();

            var advisorPrompt = BuildAdvisorPrompt(request, rankedSuggestions);
            var aiResult = await _aiRouterService.GenerateAsync("course-advisor", advisorPrompt, cancellationToken);

            var finalMessage = string.IsNullOrWhiteSpace(aiResult.Content)
                ? BuildFallbackMessage(request, rankedSuggestions, filteredCourses.Count > 0)
                : aiResult.Content;

            return new CourseAdvisorResponseDto
            {
                Message = finalMessage,
                Model = aiResult.Model,
                Suggestions = rankedSuggestions
            };
        }

        private static int CalculateScore(
            Course course,
            string? centerAddress,
            Term? term,
            ExamBatch? batch,
            string desiredLicense,
            string preferredDistrict,
            bool needNearestCenter,
            bool needEarliestExam)
        {
            var score = 0;

            if (!string.IsNullOrWhiteSpace(desiredLicense))
            {
                score += NormalizeText(course.LicenseType.ToString()) == desiredLicense ? 60 : -20;
            }

            if (!string.IsNullOrWhiteSpace(preferredDistrict) &&
                NormalizeText(centerAddress).Contains(preferredDistrict, StringComparison.OrdinalIgnoreCase))
            {
                score += needNearestCenter ? 25 : 15;
            }

            if (term != null)
            {
                score += 20;
                var remainingSeats = term.MaxStudents - term.CurrentStudents;
                if (remainingSeats > 0)
                {
                    score += Math.Min(10, remainingSeats);
                }

                var daysUntilStart = (term.StartDate.Date - DateTime.UtcNow.Date).TotalDays;
                if (daysUntilStart >= 0)
                {
                    score += daysUntilStart <= 30 ? 12 : daysUntilStart <= 60 ? 8 : 4;
                }
            }

            if (batch != null)
            {
                score += 10;
                if (batch.Status == ExamBatchStatus.OpenForRegistration)
                {
                    score += needEarliestExam ? 15 : 8;
                }

                var remainingCandidates = batch.MaxCandidates - batch.CurrentCandidates;
                if (remainingCandidates > 0)
                {
                    score += Math.Min(8, remainingCandidates / 5 + 1);
                }
            }

            return score;
        }

        private static string BuildSummary(
            Course course,
            string? centerName,
            string? centerAddress,
            Term? term,
            ExamBatch? batch,
            Exam? exam,
            Address? address)
        {
            var parts = new List<string>
            {
                $"{course.CourseName} dành cho hạng {course.LicenseType} tại {centerName ?? "trung tâm phù hợp"}."
            };

            if (!string.IsNullOrWhiteSpace(centerAddress))
            {
                parts.Add($"Địa chỉ trung tâm: {centerAddress}.");
            }

            if (term != null)
            {
                parts.Add(
                    $"Kỳ gần nhất là {term.TermName} từ {term.StartDate:dd/MM/yyyy} đến {term.EndDate:dd/MM/yyyy}, còn {Math.Max(0, term.MaxStudents - term.CurrentStudents)} chỗ.");
            }
            else
            {
                parts.Add("Hiện chưa thấy kỳ học còn chỗ trong dữ liệu gần.");
            }

            if (exam != null)
            {
                parts.Add(
                    $"Lịch thi gợi ý: {exam.ExamName} vào {exam.ExamDate:dd/MM/yyyy}" +
                    $"{(batch != null ? $" thuộc đợt {batch.BatchName}" : string.Empty)}" +
                    $"{(address != null ? $" tại {address.AddressName}" : string.Empty)}.");
            }
            else
            {
                parts.Add("Chưa có lịch thi đã xếp cho khóa này trong dữ liệu hiện tại.");
            }

            parts.Add($"Học phí tham khảo: {course.Price.ToString("N0", CultureInfo.InvariantCulture)} VNĐ.");
            return string.Join(" ", parts);
        }

        private static string BuildReason(
            Course course,
            string? centerAddress,
            Term? term,
            ExamBatch? batch,
            string preferredDistrict)
        {
            var reasons = new List<string>();

            if (!string.IsNullOrWhiteSpace(preferredDistrict) &&
                NormalizeText(centerAddress).Contains(preferredDistrict, StringComparison.OrdinalIgnoreCase))
            {
                reasons.Add("Trung tâm khá khớp với khu vực bạn ưu tiên");
            }

            reasons.Add($"Hạng bằng phù hợp với nhu cầu {course.LicenseType}");

            if (term != null)
            {
                reasons.Add($"Có kỳ học gần là {term.TermName}");
            }

            if (batch?.Status == ExamBatchStatus.OpenForRegistration)
            {
                reasons.Add("Đang có đợt thi mở đăng ký liên quan");
            }

            return string.Join(". ", reasons) + ".";
        }

        private static string BuildAdvisorPrompt(
            CourseAdvisorRequestDto request,
            IReadOnlyCollection<CourseAdvisorSuggestionDto> suggestions)
        {
            var suggestionLines = suggestions.Select((item, index) =>
                $"{index + 1}. {item.CourseName} | GPLX {item.LicenseType} | Trung tam: {item.CenterName} | Dia chi: {item.CenterAddress} | Ky hoc: {item.TermName ?? "chua co"} | Thi: {item.ExamBatchName ?? "chua co"} | Dia diem thi: {item.ExamAddressName ?? "chua co"}");

            return
                "Ban la tro ly tu van khoa hoc lai xe. " +
                "Hay viet mot doan tu van ngan gon, than thien, bang tieng Viet, dua tren du lieu that ben duoi. " +
                $"Nhu cau: hang bang={request.DesiredLicenseLevel ?? "chua ro"}, khu vuc={request.PreferredDistrict ?? "chua ro"}, lich ranh={request.PreferredSchedule ?? "chua ro"}. " +
                "Neu du lieu chua du thi noi ro dieu do, khong duoc bo sung thong tin khong co. " +
                "Du lieu goi y: " + string.Join(" ", suggestionLines);
        }

        private static string BuildFallbackMessage(
            CourseAdvisorRequestDto request,
            IReadOnlyCollection<CourseAdvisorSuggestionDto> suggestions,
            bool matchedExactLicense)
        {
            if (suggestions.Count == 0)
            {
                return "Mình chưa tìm thấy gợi ý khóa học phù hợp trong dữ liệu hiện tại. Bạn có thể thử đổi hạng bằng hoặc khu vực ưu tiên để hệ thống tư vấn lại.";
            }

            var first = suggestions.First();
            var licenseNote = matchedExactLicense
                ? $"Mình đã ưu tiên đúng hạng bằng {request.DesiredLicenseLevel}."
                : "Hiện chưa có khóa khớp hoàn toàn với hạng bằng bạn nhập, nên mình đang gợi ý các lựa chọn gần nhất.";

            return $"{licenseNote} Gợi ý nổi bật nhất lúc này là {first.CourseName} tại {first.CenterName}. " +
                   $"{(first.TermName != null ? $"Kỳ học gần nhất là {first.TermName}." : "Khóa này hiện chưa có kỳ học còn chỗ trong dữ liệu gần.")} " +
                   $"{(first.ExamAddressName != null ? $"Địa điểm thi gợi ý là {first.ExamAddressName}." : "Hiện chưa có địa điểm thi cụ thể được gắn cho khóa này.")}";
        }

        private static string NormalizeText(string? value)
            => (value ?? string.Empty).Trim().Replace(" ", string.Empty).ToUpperInvariant();
    }
}
