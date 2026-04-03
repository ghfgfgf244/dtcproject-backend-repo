using dtc.Application.Features.Email.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Location;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.Exams.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public ExamService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        public async Task<ExamResponseDto> CreateExamAsync(CreateExamRequestDto request, Guid adminId)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(request.ExamBatchId);
            if (batch == null) throw new KeyNotFoundException("Exam batch not found");

            var address = await GetAddressOrThrowAsync(request.AddressId);

            var exam = new Exam(
                examBatchId: request.ExamBatchId,
                courseId: request.CourseId,
                addressId: request.AddressId,
                name: request.ExamName,
                examDate: request.ExamDate,
                examType: request.ExamType,
                durationMinutes: request.DurationMinutes,
                totalScore: request.TotalScore,
                passScore: request.PassScore,
                createdBy: adminId);

            await _unitOfWork.Exams.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            await NotifyApprovedStudentsInBatchAsync(
                request.ExamBatchId,
                "Lich thi moi",
                $"Ky thi '{exam.ExamName}' se dien ra vao ngay {exam.ExamDate:dd/MM/yyyy}.");

            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            return MapToDto(exam, address, course?.LicenseType);
        }

        public async Task<ExamResponseDto> UpdateExamAsync(Guid id, UpdateExamRequestDto request, Guid adminId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new KeyNotFoundException("Exam not found");

            Address? address = null;
            if (request.AddressId.HasValue)
            {
                address = await GetAddressOrThrowAsync(request.AddressId.Value);
            }

            exam.UpdateInfo(
                name: request.ExamName,
                addressId: request.AddressId,
                durationMinutes: request.DurationMinutes,
                examType: request.ExamType,
                updatedBy: adminId);

            string? rescheduleMessage = null;
            if (request.ExamDate.HasValue && request.ExamDate.Value != exam.ExamDate)
            {
                var oldDate = exam.ExamDate;
                exam.Schedule(request.ExamDate.Value, adminId);
                rescheduleMessage = $"Ky thi '{exam.ExamName}' doi tu {oldDate:dd/MM/yyyy} sang {exam.ExamDate:dd/MM/yyyy}.";
            }

            if (request.TotalScore.HasValue && request.PassScore.HasValue &&
                (request.TotalScore.Value != exam.TotalScore || request.PassScore.Value != exam.PassScore))
            {
                exam.ChangeScoreRule(request.TotalScore.Value, request.PassScore.Value, adminId);
            }

            if (request.Status.HasValue && request.Status.Value != exam.Status)
            {
                switch (request.Status.Value)
                {
                    case ExamStatus.Finished:
                        exam.Finish(adminId);
                        break;
                    case ExamStatus.Cancelled:
                        exam.Cancel(adminId);
                        break;
                }
            }

            await _unitOfWork.Exams.UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(rescheduleMessage))
            {
                await NotifyApprovedStudentsInBatchAsync(
                    exam.ExamBatchId,
                    "Thay doi lich thi",
                    rescheduleMessage);
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(exam.CourseId);
            address ??= await _unitOfWork.Addresses.GetByIdAsync(exam.AddressId);
            return MapToDto(exam, address, course?.LicenseType);
        }

        public async Task<bool> DeleteExamAsync(Guid id, Guid adminId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new KeyNotFoundException("Exam not found");

            exam.SoftDelete(adminId);
            await _unitOfWork.Exams.UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<ExamResponseDto> GetExamDetailAsync(Guid id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new KeyNotFoundException("Exam not found");

            var course = await _unitOfWork.Courses.GetByIdAsync(exam.CourseId);
            var address = await _unitOfWork.Addresses.GetByIdAsync(exam.AddressId);
            return MapToDto(exam, address, course?.LicenseType);
        }

        public async Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync()
        {
            var exams = await _unitOfWork.Exams.GetAllAsync();
            var courseIds = exams.Select(e => e.CourseId).Distinct().ToList();
            var addressIds = exams.Select(e => e.AddressId).Distinct().ToList();
            var courses = await _unitOfWork.Courses.FindAsync(c => courseIds.Contains(c.Id));
            var addresses = await _unitOfWork.Addresses.FindAsync(a => addressIds.Contains(a.Id));
            var courseMap = courses.ToDictionary(c => c.Id);
            var addressMap = addresses.ToDictionary(a => a.Id);

            return exams.Select(e => MapToDto(
                e,
                addressMap.GetValueOrDefault(e.AddressId),
                courseMap.GetValueOrDefault(e.CourseId)?.LicenseType)).ToList();
        }

        public async Task<IEnumerable<ExamResponseDto>> GetExamsByCourseAsync(Guid courseId)
        {
            var exams = await _unitOfWork.Exams.FindAsync(e => e.CourseId == courseId);
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            var addressIds = exams.Select(e => e.AddressId).Distinct().ToList();
            var addresses = await _unitOfWork.Addresses.FindAsync(a => addressIds.Contains(a.Id));
            var addressMap = addresses.ToDictionary(a => a.Id);

            return exams.Select(e => MapToDto(
                e,
                addressMap.GetValueOrDefault(e.AddressId),
                course?.LicenseType)).ToList();
        }

        public async Task<IEnumerable<ExamResponseDto>> GetExamsByBatchAsync(Guid batchId)
        {
            var exams = await _unitOfWork.Exams.FindAsync(e => e.ExamBatchId == batchId);
            var courseIds = exams.Select(e => e.CourseId).Distinct().ToList();
            var addressIds = exams.Select(e => e.AddressId).Distinct().ToList();
            var courses = await _unitOfWork.Courses.FindAsync(c => courseIds.Contains(c.Id));
            var addresses = await _unitOfWork.Addresses.FindAsync(a => addressIds.Contains(a.Id));
            var courseMap = courses.ToDictionary(c => c.Id);
            var addressMap = addresses.ToDictionary(a => a.Id);

            return exams.Select(e => MapToDto(
                e,
                addressMap.GetValueOrDefault(e.AddressId),
                courseMap.GetValueOrDefault(e.CourseId)?.LicenseType)).ToList();
        }

        public async Task<object> GetExamResultsAsync(Guid examId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null) throw new KeyNotFoundException("Exam not found");

            var batchId = exam.ExamBatchId;
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(batchId);
            var allExamsInBatch = await _unitOfWork.Exams.FindAsync(e => e.ExamBatchId == batchId);
            var examIds = allExamsInBatch.Select(e => e.Id).ToList();
            var examTypeMap = allExamsInBatch.ToDictionary(e => e.Id, e => e.ExamType);
            var allResults = await _unitOfWork.ExamResults.FindAsync(r => examIds.Contains(r.ExamId));

            var studentIds = allResults.Select(r => r.StudentId).Distinct().ToList();
            var students = await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id));
            var studentMap = students.ToDictionary(u => u.Id);

            var studentReports = allResults
                .GroupBy(r => r.StudentId)
                .Select(g =>
                {
                    var studentId = g.Key;
                    var student = studentMap.GetValueOrDefault(studentId);
                    var studentResults = g.ToList();

                    var theoryResult = studentResults
                        .Where(r => examTypeMap[r.ExamId] == ExamType.Theory)
                        .OrderByDescending(r => r.Score)
                        .FirstOrDefault();

                    var simulationResult = studentResults
                        .Where(r => examTypeMap[r.ExamId] == ExamType.Simulation)
                        .OrderByDescending(r => r.Score)
                        .FirstOrDefault();

                    var practiceResult = studentResults
                        .Where(r => examTypeMap[r.ExamId] == ExamType.Practice)
                        .OrderByDescending(r => r.Score)
                        .FirstOrDefault();

                    return new
                    {
                        StudentId = studentId,
                        FullName = student?.FullName ?? "N/A",
                        TheoryScore = theoryResult?.Score,
                        TheoryPassed = theoryResult?.IsPassed ?? false,
                        SimulationScore = simulationResult?.Score,
                        SimulationPassed = simulationResult?.IsPassed ?? false,
                        PracticeScore = practiceResult?.Score,
                        PracticePassed = practiceResult?.IsPassed ?? false,
                        IsFullyQualified = (theoryResult?.IsPassed == true)
                            && (simulationResult?.IsPassed == true)
                            && (practiceResult?.IsPassed == true)
                    };
                })
                .ToList();

            return new
            {
                BatchId = batchId,
                BatchName = batch?.BatchName,
                TotalStudents = studentReports.Count,
                Results = studentReports
            };
        }

        public async Task<bool> UpdateExamResultAsync(Guid resultId, UpdateExamResultRequestDto request, Guid adminId)
        {
            var result = await _unitOfWork.ExamResults.GetByIdAsync(resultId);
            if (result == null) throw new KeyNotFoundException("Exam result not found");

            var exam = await _unitOfWork.Exams.GetByIdAsync(result.ExamId);
            if (exam == null) throw new KeyNotFoundException("Exam not found");

            result.Grade(request.Score, exam.PassScore);
            await _unitOfWork.ExamResults.UpdateAsync(result);
            await _unitOfWork.SaveChangesAsync();

            await NotifyStudentResultAsync(result, exam);
            return true;
        }

        public async Task<bool> EnterBulkExamResultsAsync(BulkExamResultRequestDto request, Guid adminId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(request.ExamId);
            if (exam == null) throw new KeyNotFoundException("Exam not found");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var studentIds = request.Results.Select(r => r.StudentId).Distinct().ToList();
                var students = await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id));
                if (students.Count() != studentIds.Count)
                    throw new KeyNotFoundException("One or more students not found");

                var existingResults = await _unitOfWork.ExamResults.FindAsync(
                    r => r.ExamId == request.ExamId && studentIds.Contains(r.StudentId));
                var resultsByStudent = existingResults.ToDictionary(r => r.StudentId);

                var processedResults = new List<ExamResult>();

                foreach (var resDto in request.Results)
                {
                    if (resultsByStudent.TryGetValue(resDto.StudentId, out var existing))
                    {
                        existing.Grade(resDto.Score, exam.PassScore);
                        await _unitOfWork.ExamResults.UpdateAsync(existing);
                        processedResults.Add(existing);
                    }
                    else
                    {
                        var newResult = new ExamResult(request.ExamId, resDto.StudentId, 1, DateTime.UtcNow);
                        newResult.Grade(resDto.Score, exam.PassScore);
                        await _unitOfWork.ExamResults.AddAsync(newResult);
                        processedResults.Add(newResult);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                foreach (var processed in processedResults)
                {
                    await NotifyStudentResultAsync(processed, exam);
                }

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<object> GetMyExamResultsAsync(Guid studentId)
        {
            var results = await _unitOfWork.ExamResults.FindAsync(r => r.StudentId == studentId);
            if (!results.Any()) return new List<object>();

            var examIds = results.Select(r => r.ExamId).Distinct().ToList();
            var exams = await _unitOfWork.Exams.FindAsync(e => examIds.Contains(e.Id));
            var examMap = exams.ToDictionary(e => e.Id);

            var bestResults = results
                .Where(r => examMap.ContainsKey(r.ExamId))
                .GroupBy(r => examMap[r.ExamId].ExamType)
                .Select(g =>
                {
                    var best = g.OrderByDescending(r => r.Score).First();
                    var exam = examMap[best.ExamId];
                    return new
                    {
                        ExamId = best.ExamId,
                        ExamName = exam.ExamName,
                        ExamType = exam.ExamType.ToString(),
                        Score = best.Score,
                        IsPassed = best.IsPassed,
                        ExamDate = exam.ExamDate,
                        TotalScore = exam.TotalScore,
                        PassScore = exam.PassScore
                    };
                })
                .ToList();

            return bestResults;
        }

        public async Task<IEnumerable<ExamResponseDto>> GetMyExamsAsync(Guid studentId)
        {
            var registrations = await _unitOfWork.ExamRegistrations.FindAsync(
                r => r.StudentId == studentId && r.Status == ExamRegistrationStatus.Approved);
            if (!registrations.Any()) return new List<ExamResponseDto>();

            var batchIds = registrations.Select(r => r.ExamBatchId).Distinct().ToList();

            var courseRegs = await _unitOfWork.CourseRegistrations.FindAsync(
                r => r.UserId == studentId && r.Status == CourseRegistrationStatus.Approved);
            if (!courseRegs.Any()) return new List<ExamResponseDto>();

            var courseIds = courseRegs.Select(r => r.CourseId).Distinct().ToList();
            var exams = await _unitOfWork.Exams.FindAsync(
                e => batchIds.Contains(e.ExamBatchId) && courseIds.Contains(e.CourseId));
            if (!exams.Any()) return new List<ExamResponseDto>();

            var addressIds = exams.Select(e => e.AddressId).Distinct().ToList();
            var addresses = await _unitOfWork.Addresses.FindAsync(a => addressIds.Contains(a.Id));
            var addressMap = addresses.ToDictionary(a => a.Id);

            var courses = await _unitOfWork.Courses.FindAsync(c => courseIds.Contains(c.Id));
            var courseMap = courses.ToDictionary(c => c.Id);

            return exams.Select(e => MapToDto(
                    e,
                    addressMap.GetValueOrDefault(e.AddressId),
                    courseMap.GetValueOrDefault(e.CourseId)?.LicenseType))
                .OrderBy(e => e.ExamDate)
                .ToList();
        }

        private async Task NotifyStudentResultAsync(ExamResult result, Exam exam)
        {
            try
            {
                var student = await _unitOfWork.Users.GetByIdAsync(result.StudentId);
                if (student == null)
                {
                    return;
                }

                var statusText = result.IsPassed ? "DAT" : "KHONG DAT";
                await _notificationService.CreateForUserAsync(
                    result.StudentId,
                    "Ket qua thi",
                    $"Ky thi '{exam.ExamName}': diem {result.Score:0.##} - {statusText}.",
                    NotificationType.ExamResult);

                await _emailService.SendExamResultAsync(
                    student.Email.Value,
                    student.FullName,
                    exam.ExamName,
                    result.Score,
                    result.IsPassed);
            }
            catch
            {
            }
        }

        private async Task NotifyApprovedStudentsInBatchAsync(Guid examBatchId, string title, string content)
        {
            try
            {
                var registrations = await _unitOfWork.ExamRegistrations.FindAsync(
                    r => r.ExamBatchId == examBatchId && r.Status == ExamRegistrationStatus.Approved);

                foreach (var registration in registrations)
                {
                    try
                    {
                        await _notificationService.CreateForUserAsync(
                            registration.StudentId,
                            title,
                            content,
                            NotificationType.Exam);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        private async Task<Address> GetAddressOrThrowAsync(int addressId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);
            if (address == null)
            {
                throw new KeyNotFoundException($"Address with id {addressId} was not found.");
            }

            return address;
        }

        private ExamResponseDto MapToDto(Exam exam, Address? address, ExamLevel? licenseType = null)
        {
            return new ExamResponseDto
            {
                Id = exam.Id,
                ExamBatchId = exam.ExamBatchId,
                CourseId = exam.CourseId,
                AddressId = exam.AddressId,
                AddressName = address?.AddressName ?? string.Empty,
                ExamName = exam.ExamName,
                ExamDate = exam.ExamDate,
                ExamType = exam.ExamType,
                DurationMinutes = exam.DurationMinutes,
                TotalScore = exam.TotalScore,
                PassScore = exam.PassScore,
                LicenseType = licenseType,
                Status = exam.Status,
                CreatedAt = exam.CreatedAt
            };
        }
    }
}
