using dtc.Application.Features.Email.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Entities.Location;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Entities.Terms;
using dtc.Domain.Entities.Training;
using dtc.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO.Compression;
using System.Security;
using System.Text;
using System.Xml.Linq;

namespace dtc.Application.Features.Exams.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ExamService> _logger;

        public ExamService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService,
            ILogger<ExamService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
            _logger = logger;
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
            return MapToDto(exam, address, course?.LicenseType, course?.CenterId);
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
            return MapToDto(exam, address, course?.LicenseType, course?.CenterId);
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
            return MapToDto(exam, address, course?.LicenseType, course?.CenterId);
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
                courseMap.GetValueOrDefault(e.CourseId)?.LicenseType,
                courseMap.GetValueOrDefault(e.CourseId)?.CenterId)).ToList();
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
                course?.LicenseType,
                course?.CenterId)).ToList();
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
                courseMap.GetValueOrDefault(e.CourseId)?.LicenseType,
                courseMap.GetValueOrDefault(e.CourseId)?.CenterId)).ToList();
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

            var processedResults = await _unitOfWork.ExecuteInTransactionAsync(async () =>
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
                return processedResults;
            });

            foreach (var processed in processedResults)
            {
                await NotifyStudentResultAsync(processed, exam);
            }

            return true;
        }

        public async Task<ExamScoreboardResponseDto> GetExamScoreboardAsync(ExamScoreboardQueryDto query)
        {
            var items = await BuildScoreboardItemsAsync(query);

            var totalItems = items.Count;
            var totalPassed = items.Count(item => item.IsPassedAll);
            var averageOverallScore = totalItems == 0
                ? 0
                : decimal.Round(items.Average(item => item.OverallScore), 2);
            var hasSimulationExam = items.Any(item => item.HasSimulationExam);

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Max(1, Math.Min(query.PageSize, 100));
            var pagedItems = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ExamScoreboardResponseDto
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize),
                TotalPassed = totalPassed,
                AverageOverallScore = averageOverallScore,
                HasSimulationExam = hasSimulationExam,
                Items = pagedItems
            };
        }

        public async Task<ExamScoreboardItemDto> UpsertStudentExamScoresAsync(UpsertStudentExamScoresRequestDto request, Guid adminId)
        {
            var context = await LoadSelectionContextAsync(request.CourseId, request.TermId, request.ExamBatchId);
            var candidate = await GetExactCandidateAsync(request.CourseId, request.TermId, request.ExamBatchId, request.StudentId);
            if (candidate == null)
            {
                throw new InvalidOperationException("Student is not eligible to receive scores for the selected course, term, and exam batch.");
            }

            var examsByType = context.Exams
                .GroupBy(exam => exam.ExamType)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(exam => exam.ExamDate).First());

            var processedResults = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var examIds = context.Exams.Select(exam => exam.Id).ToList();
                var existingResults = await _unitOfWork.ExamResults.FindAsync(
                    result => examIds.Contains(result.ExamId) && result.StudentId == request.StudentId);
                var existingMap = existingResults.ToDictionary(result => result.ExamId);
                var touched = new List<ExamResult>();

                async Task UpsertComponentAsync(ExamType examType, double? score)
                {
                    if (!score.HasValue || !examsByType.TryGetValue(examType, out var exam))
                    {
                        return;
                    }

                    if (existingMap.TryGetValue(exam.Id, out var existingResult))
                    {
                        existingResult.Grade(score.Value, exam.PassScore);
                        await _unitOfWork.ExamResults.UpdateAsync(existingResult);
                        touched.Add(existingResult);
                        return;
                    }

                    var newResult = new ExamResult(exam.Id, request.StudentId, 1, DateTime.UtcNow);
                    newResult.Grade(score.Value, exam.PassScore);
                    await _unitOfWork.ExamResults.AddAsync(newResult);
                    touched.Add(newResult);
                }

                await UpsertComponentAsync(ExamType.Theory, request.TheoryScore);
                await UpsertComponentAsync(ExamType.Practice, request.PracticeScore);
                await UpsertComponentAsync(ExamType.Simulation, request.SimulationScore);

                await _unitOfWork.SaveChangesAsync();
                return touched;
            });

            foreach (var processedResult in processedResults)
            {
                var exam = context.Exams.First(ex => ex.Id == processedResult.ExamId);
                await NotifyStudentResultAsync(processedResult, exam);
            }

            var refreshedResults = await _unitOfWork.ExamResults.FindAsync(
                result => context.ExamIds.Contains(result.ExamId) && result.StudentId == request.StudentId);

            return BuildScoreboardItem(
                candidate.Student,
                candidate.CourseRegistration,
                context.Course,
                context.Term,
                context.Batch,
                context.Exams,
                refreshedResults.ToList());
        }

        public async Task<ExamScoreImportResponseDto> ImportExamScoresAsync(ExamScoreImportRequestDto request, IFormFile file, Guid adminId)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("Score import file is required.");
            }

            await LoadSelectionContextAsync(request.CourseId, request.TermId, request.ExamBatchId);

            List<Dictionary<string, string>> rows;
            await using (var stream = file.OpenReadStream())
            {
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                rows = extension switch
                {
                    ".csv" => await ReadCsvRowsAsync(stream),
                    ".xlsx" => await ReadXlsxRowsAsync(stream),
                    _ => throw new InvalidOperationException("Only .xlsx or .csv files are supported.")
                };
            }

            var candidateMap = (await GetExactCandidatesAsync(request.CourseId, request.TermId, request.ExamBatchId))
                .ToDictionary(candidate => candidate.Student.Id, candidate => candidate);

            var response = new ExamScoreImportResponseDto();
            foreach (var row in rows)
            {
                var rowLabel = $"Row {row.GetValueOrDefault("__row") ?? "?"}";
                try
                {
                    var studentIdText = GetRequiredValue(row, rowLabel, "studentid", "userid", "studentguid");
                    if (!Guid.TryParse(studentIdText, out var studentId))
                    {
                        throw new InvalidOperationException($"{rowLabel}: StudentId must be a valid GUID.");
                    }

                    if (!candidateMap.ContainsKey(studentId))
                    {
                        throw new InvalidOperationException($"{rowLabel}: Student does not belong to the selected course, term, and exam batch.");
                    }

                    var theoryScore = GetOptionalScore(row, rowLabel, "theoryscore", "lythuyet", "theory");
                    var practiceScore = GetOptionalScore(row, rowLabel, "practicescore", "thuchanh", "practice");
                    var simulationScore = GetOptionalScore(row, rowLabel, "simulationscore", "mophong", "simulation");

                    if (!theoryScore.HasValue && !practiceScore.HasValue && !simulationScore.HasValue)
                    {
                        response.Warnings.Add($"{rowLabel}: skipped because all score columns are empty.");
                        continue;
                    }

                    await UpsertStudentExamScoresAsync(
                        new UpsertStudentExamScoresRequestDto
                        {
                            CourseId = request.CourseId,
                            TermId = request.TermId,
                            ExamBatchId = request.ExamBatchId,
                            StudentId = studentId,
                            TheoryScore = theoryScore,
                            PracticeScore = practiceScore,
                            SimulationScore = simulationScore
                        },
                        adminId);

                    response.ImportedCount++;
                }
                catch (Exception ex)
                {
                    response.Warnings.Add(ex.Message);
                }
            }

            return response;
        }

        public async Task<byte[]> GenerateScoreImportTemplateAsync(Guid courseId, Guid termId, Guid examBatchId)
        {
            var context = await LoadSelectionContextAsync(courseId, termId, examBatchId);
            var candidates = await GetExactCandidatesAsync(courseId, termId, examBatchId);
            var candidateStudentIds = candidates.Select(candidate => candidate.Student.Id).Distinct().ToList();
            var existingResults = await _unitOfWork.ExamResults.FindAsync(
                result => context.ExamIds.Contains(result.ExamId) && candidateStudentIds.Contains(result.StudentId));
            var resultMap = existingResults
                .GroupBy(result => BuildResultKey(result.StudentId, result.ExamId))
                .ToDictionary(group => group.Key, group => group.OrderByDescending(item => item.Score).First());

            var headers = new List<string>
            {
                "StudentId",
                "StudentName",
                "Email",
                "Phone",
                "TheoryScore",
                "PracticeScore"
            };

            if (context.HasSimulationExam)
            {
                headers.Add("SimulationScore");
            }

            var rows = new List<string[]>();
            foreach (var candidate in candidates.OrderBy(item => item.Student.FullName))
            {
                var row = new List<string>
                {
                    candidate.Student.Id.ToString(),
                    candidate.Student.FullName,
                    candidate.Student.Email.Value,
                    candidate.Student.Phone.Value
                };

                row.Add(GetExistingScoreText(resultMap, candidate.Student.Id, context.ExamsByType, ExamType.Theory));
                row.Add(GetExistingScoreText(resultMap, candidate.Student.Id, context.ExamsByType, ExamType.Practice));
                if (context.HasSimulationExam)
                {
                    row.Add(GetExistingScoreText(resultMap, candidate.Student.Id, context.ExamsByType, ExamType.Simulation));
                }

                rows.Add(row.ToArray());
            }

            return BuildXlsxFile("ExamScores", headers, rows);
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

        private async Task<List<ExamScoreboardItemDto>> BuildScoreboardItemsAsync(ExamScoreboardQueryDto query)
        {
            var approvedCourseRegistrations = (await _unitOfWork.CourseRegistrations.FindAsync(
                    registration =>
                        registration.Status == CourseRegistrationStatus.Approved &&
                        registration.AssignedTermId.HasValue &&
                        (!query.CourseId.HasValue || registration.CourseId == query.CourseId.Value) &&
                        (!query.TermId.HasValue || registration.AssignedTermId == query.TermId.Value)))
                .GroupBy(registration => new
                {
                    registration.UserId,
                    registration.CourseId,
                    TermId = registration.AssignedTermId!.Value
                })
                .Select(group => group.OrderByDescending(item => item.RegistrationDate).First())
                .ToList();

            if (approvedCourseRegistrations.Count == 0)
            {
                return new List<ExamScoreboardItemDto>();
            }

            var studentIds = approvedCourseRegistrations.Select(registration => registration.UserId).Distinct().ToList();
            var courseIds = approvedCourseRegistrations.Select(registration => registration.CourseId).Distinct().ToList();
            var termIds = approvedCourseRegistrations
                .Where(registration => registration.AssignedTermId.HasValue)
                .Select(registration => registration.AssignedTermId!.Value)
                .Distinct()
                .ToList();

            var examRegistrations = (await _unitOfWork.ExamRegistrations.FindAsync(
                    registration =>
                        studentIds.Contains(registration.StudentId) &&
                        registration.Status == ExamRegistrationStatus.Approved &&
                        (!query.ExamBatchId.HasValue || registration.ExamBatchId == query.ExamBatchId.Value)))
                .ToList();

            if (examRegistrations.Count == 0)
            {
                return new List<ExamScoreboardItemDto>();
            }

            var batchIds = examRegistrations.Select(registration => registration.ExamBatchId).Distinct().ToList();
            var exams = (await _unitOfWork.Exams.FindAsync(
                    exam => batchIds.Contains(exam.ExamBatchId) && courseIds.Contains(exam.CourseId)))
                .ToList();

            if (exams.Count == 0)
            {
                return new List<ExamScoreboardItemDto>();
            }

            var examIds = exams.Select(exam => exam.Id).Distinct().ToList();
            var users = await _unitOfWork.Users.FindAsync(user => studentIds.Contains(user.Id));
            var courses = await _unitOfWork.Courses.FindAsync(course => courseIds.Contains(course.Id));
            var terms = await _unitOfWork.Terms.FindAsync(term => termIds.Contains(term.Id));
            var batches = await _unitOfWork.ExamBatches.FindAsync(batch => batchIds.Contains(batch.Id));
            var results = await _unitOfWork.ExamResults.FindAsync(result => examIds.Contains(result.ExamId) && studentIds.Contains(result.StudentId));

            var userMap = users.ToDictionary(user => user.Id);
            var courseMap = courses.ToDictionary(course => course.Id);
            var termMap = terms.ToDictionary(term => term.Id);
            var batchMap = batches.ToDictionary(batch => batch.Id);
            var examsByBatchAndCourse = exams
                .GroupBy(exam => new { exam.ExamBatchId, exam.CourseId })
                .ToDictionary(
                    group => $"{group.Key.ExamBatchId:N}_{group.Key.CourseId:N}",
                    group => group.ToList());

            var resultsByStudent = results
                .GroupBy(result => result.StudentId)
                .ToDictionary(group => group.Key, group => group.ToList());

            var items = new List<ExamScoreboardItemDto>();
            foreach (var registration in approvedCourseRegistrations)
            {
                if (!registration.AssignedTermId.HasValue)
                {
                    continue;
                }

                if (!userMap.TryGetValue(registration.UserId, out var user) ||
                    !courseMap.TryGetValue(registration.CourseId, out var course) ||
                    !termMap.TryGetValue(registration.AssignedTermId.Value, out var term))
                {
                    continue;
                }

                foreach (var examRegistration in examRegistrations.Where(item => item.StudentId == registration.UserId))
                {
                    var key = $"{examRegistration.ExamBatchId:N}_{registration.CourseId:N}";
                    if (!batchMap.TryGetValue(examRegistration.ExamBatchId, out var batch) ||
                        !examsByBatchAndCourse.TryGetValue(key, out var rowExams))
                    {
                        continue;
                    }

                    var rowResults = resultsByStudent.GetValueOrDefault(registration.UserId)?
                        .Where(result => rowExams.Any(exam => exam.Id == result.ExamId))
                        .ToList() ?? new List<ExamResult>();

                    items.Add(BuildScoreboardItem(user, registration, course, term, batch, rowExams, rowResults));
                }
            }

            var filtered = items
                .GroupBy(item => new { item.StudentId, item.CourseId, item.TermId, item.ExamBatchId })
                .Select(group => group.First())
                .ToList();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim();
                filtered = filtered
                    .Where(item =>
                        item.StudentName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        item.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        item.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            filtered = string.Equals(query.SortDirection, "asc", StringComparison.OrdinalIgnoreCase)
                ? filtered.OrderBy(item => item.OverallScore).ThenBy(item => item.StudentName).ToList()
                : filtered.OrderByDescending(item => item.OverallScore).ThenBy(item => item.StudentName).ToList();

            return filtered;
        }

        private ExamScoreboardItemDto BuildScoreboardItem(
            User student,
            CourseRegistration registration,
            Course course,
            Term term,
            ExamBatch batch,
            IReadOnlyCollection<Exam> rowExams,
            IReadOnlyCollection<ExamResult> rowResults)
        {
            var examsByType = rowExams
                .GroupBy(exam => exam.ExamType)
                .ToDictionary(group => group.Key, group => group.ToList());

            double? theoryScore = GetComponentScore(rowResults, examsByType, ExamType.Theory);
            double? practiceScore = GetComponentScore(rowResults, examsByType, ExamType.Practice);
            double? simulationScore = GetComponentScore(rowResults, examsByType, ExamType.Simulation);

            var theoryPassed = GetComponentPassed(rowResults, examsByType, ExamType.Theory) ?? false;
            var practicePassed = GetComponentPassed(rowResults, examsByType, ExamType.Practice) ?? false;
            var simulationPassed = GetComponentPassed(rowResults, examsByType, ExamType.Simulation);

            var normalizedScores = new List<decimal>();
            AddNormalizedScore(normalizedScores, rowResults, examsByType, ExamType.Theory);
            AddNormalizedScore(normalizedScores, rowResults, examsByType, ExamType.Practice);
            AddNormalizedScore(normalizedScores, rowResults, examsByType, ExamType.Simulation);

            var totalComponents = 0;
            if (examsByType.ContainsKey(ExamType.Theory)) totalComponents++;
            if (examsByType.ContainsKey(ExamType.Practice)) totalComponents++;
            if (examsByType.ContainsKey(ExamType.Simulation)) totalComponents++;

            var completedComponents = 0;
            if (theoryScore.HasValue) completedComponents++;
            if (practiceScore.HasValue) completedComponents++;
            if (simulationScore.HasValue) completedComponents++;

            var hasSimulationExam = examsByType.ContainsKey(ExamType.Simulation);
            var isPassedAll = theoryPassed &&
                              practicePassed &&
                              (!hasSimulationExam || simulationPassed == true);

            return new ExamScoreboardItemDto
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                Email = student.Email.Value,
                Phone = student.Phone.Value,
                CourseId = registration.CourseId,
                CourseName = course.CourseName,
                LicenseTypeLabel = course.LicenseType.ToString(),
                TermId = registration.AssignedTermId!.Value,
                TermName = term.TermName,
                ExamBatchId = batch.Id,
                ExamBatchName = batch.BatchName,
                HasSimulationExam = hasSimulationExam,
                TheoryScore = theoryScore,
                PracticeScore = practiceScore,
                SimulationScore = simulationScore,
                TheoryPassed = theoryPassed,
                PracticePassed = practicePassed,
                SimulationPassed = hasSimulationExam ? simulationPassed : null,
                OverallScore = normalizedScores.Count == 0 ? 0 : decimal.Round(normalizedScores.Average(), 2),
                IsPassedAll = isPassedAll,
                CompletedComponents = completedComponents,
                TotalComponents = totalComponents
            };
        }

        private static void AddNormalizedScore(
            List<decimal> normalizedScores,
            IReadOnlyCollection<ExamResult> rowResults,
            IReadOnlyDictionary<ExamType, List<Exam>> examsByType,
            ExamType examType)
        {
            if (!examsByType.TryGetValue(examType, out var examsForType))
            {
                return;
            }

            var matchingResults = rowResults
                .Where(result => examsForType.Any(exam => exam.Id == result.ExamId))
                .ToList();

            if (matchingResults.Count == 0)
            {
                return;
            }

            var bestResult = matchingResults.OrderByDescending(result => result.Score).First();
            var exam = examsForType.First(item => item.Id == bestResult.ExamId);
            if (exam.TotalScore <= 0)
            {
                return;
            }

            normalizedScores.Add(decimal.Round((decimal)(bestResult.Score / exam.TotalScore * 100d), 2));
        }

        private static double? GetComponentScore(
            IReadOnlyCollection<ExamResult> rowResults,
            IReadOnlyDictionary<ExamType, List<Exam>> examsByType,
            ExamType examType)
        {
            if (!examsByType.TryGetValue(examType, out var examsForType))
            {
                return null;
            }

            return rowResults
                .Where(result => examsForType.Any(exam => exam.Id == result.ExamId))
                .OrderByDescending(result => result.Score)
                .Select(result => (double?)result.Score)
                .FirstOrDefault();
        }

        private static bool? GetComponentPassed(
            IReadOnlyCollection<ExamResult> rowResults,
            IReadOnlyDictionary<ExamType, List<Exam>> examsByType,
            ExamType examType)
        {
            if (!examsByType.TryGetValue(examType, out var examsForType))
            {
                return null;
            }

            return rowResults
                .Where(result => examsForType.Any(exam => exam.Id == result.ExamId))
                .OrderByDescending(result => result.Score)
                .Select(result => (bool?)result.IsPassed)
                .FirstOrDefault();
        }

        private async Task<ScoreSelectionContext> LoadSelectionContextAsync(Guid courseId, Guid termId, Guid examBatchId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId)
                ?? throw new KeyNotFoundException("Course not found.");
            var term = await _unitOfWork.Terms.GetByIdAsync(termId)
                ?? throw new KeyNotFoundException("Term not found.");
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(examBatchId)
                ?? throw new KeyNotFoundException("Exam batch not found.");

            if (term.CourseId != courseId)
            {
                throw new InvalidOperationException("Selected term does not belong to the selected course.");
            }

            var exams = (await _unitOfWork.Exams.FindAsync(exam => exam.ExamBatchId == examBatchId && exam.CourseId == courseId))
                .ToList();
            if (exams.Count == 0)
            {
                throw new InvalidOperationException("No exams were found for the selected course and exam batch.");
            }

            return new ScoreSelectionContext
            {
                Course = course,
                Term = term,
                Batch = batch,
                Exams = exams,
                ExamIds = exams.Select(exam => exam.Id).ToList(),
                ExamsByType = exams
                    .GroupBy(exam => exam.ExamType)
                    .ToDictionary(group => group.Key, group => group.OrderByDescending(exam => exam.ExamDate).First()),
                HasSimulationExam = exams.Any(exam => exam.ExamType == ExamType.Simulation)
            };
        }

        private async Task<ScoreEntryCandidate?> GetExactCandidateAsync(Guid courseId, Guid termId, Guid examBatchId, Guid studentId)
        {
            return (await GetExactCandidatesAsync(courseId, termId, examBatchId))
                .FirstOrDefault(candidate => candidate.Student.Id == studentId);
        }

        private async Task<List<ScoreEntryCandidate>> GetExactCandidatesAsync(Guid courseId, Guid termId, Guid examBatchId)
        {
            var courseRegistrations = (await _unitOfWork.CourseRegistrations.FindAsync(
                    registration =>
                        registration.CourseId == courseId &&
                        registration.AssignedTermId == termId &&
                        registration.Status == CourseRegistrationStatus.Approved))
                .GroupBy(registration => registration.UserId)
                .Select(group => group.OrderByDescending(item => item.RegistrationDate).First())
                .ToList();

            if (courseRegistrations.Count == 0)
            {
                return new List<ScoreEntryCandidate>();
            }

            var studentIds = courseRegistrations.Select(registration => registration.UserId).Distinct().ToList();
            var examRegistrations = (await _unitOfWork.ExamRegistrations.FindAsync(
                    registration =>
                        registration.ExamBatchId == examBatchId &&
                        studentIds.Contains(registration.StudentId) &&
                        registration.Status == ExamRegistrationStatus.Approved))
                .ToList();

            if (examRegistrations.Count == 0)
            {
                return new List<ScoreEntryCandidate>();
            }

            var users = await _unitOfWork.Users.FindAsync(user => studentIds.Contains(user.Id));
            var userMap = users.ToDictionary(user => user.Id);
            var examRegistrationMap = examRegistrations.ToDictionary(registration => registration.StudentId);

            return courseRegistrations
                .Where(registration => examRegistrationMap.ContainsKey(registration.UserId) && userMap.ContainsKey(registration.UserId))
                .Select(registration => new ScoreEntryCandidate
                {
                    CourseRegistration = registration,
                    ExamRegistration = examRegistrationMap[registration.UserId],
                    Student = userMap[registration.UserId]
                })
                .OrderBy(candidate => candidate.Student.FullName)
                .ToList();
        }

        private static string GetExistingScoreText(
            IReadOnlyDictionary<string, ExamResult> resultMap,
            Guid studentId,
            IReadOnlyDictionary<ExamType, Exam> examsByType,
            ExamType examType)
        {
            if (!examsByType.TryGetValue(examType, out var exam))
            {
                return string.Empty;
            }

            var key = BuildResultKey(studentId, exam.Id);
            return resultMap.TryGetValue(key, out var result)
                ? result.Score.ToString("0.##", CultureInfo.InvariantCulture)
                : string.Empty;
        }

        private static string BuildResultKey(Guid studentId, Guid examId)
        {
            return $"{studentId:N}_{examId:N}";
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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to notify exam result for student {StudentId}, exam {ExamId}.",
                    result.StudentId,
                    exam.Id);
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
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Failed to create exam notification for student {StudentId} in batch {ExamBatchId}.",
                            registration.StudentId,
                            examBatchId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to notify approved students in exam batch {ExamBatchId}.",
                    examBatchId);
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

        private static async Task<List<Dictionary<string, string>>> ReadCsvRowsAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, true, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1)
            {
                return new List<Dictionary<string, string>>();
            }

            var headers = SplitCsvLine(lines[0]).Select(NormalizeHeader).ToList();
            var rows = new List<Dictionary<string, string>>();

            for (var i = 1; i < lines.Length; i++)
            {
                var values = SplitCsvLine(lines[i]);
                var row = new Dictionary<string, string> { ["__row"] = (i + 1).ToString(CultureInfo.InvariantCulture) };
                for (var j = 0; j < headers.Count; j++)
                {
                    row[headers[j]] = j < values.Count ? values[j] : string.Empty;
                }

                rows.Add(row);
            }

            return rows;
        }

        private static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }

                current.Append(ch);
            }

            result.Add(current.ToString().Trim());
            return result;
        }

        private static async Task<List<Dictionary<string, string>>> ReadXlsxRowsAsync(Stream stream)
        {
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            using var archive = new ZipArchive(memory, ZipArchiveMode.Read, leaveOpen: true);
            var workbookEntry = archive.GetEntry("xl/workbook.xml")
                ?? throw new InvalidOperationException("Invalid xlsx file.");
            var workbookRelsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels")
                ?? throw new InvalidOperationException("Invalid xlsx file.");
            var sharedStringsEntry = archive.GetEntry("xl/sharedStrings.xml");

            var workbook = XDocument.Load(workbookEntry.Open());
            var workbookRels = XDocument.Load(workbookRelsEntry.Open());
            var ns = workbook.Root?.Name.Namespace ?? XNamespace.None;
            var relNs = workbookRels.Root?.Name.Namespace ?? XNamespace.None;

            var firstSheet = workbook.Descendants(ns + "sheet").FirstOrDefault()
                ?? throw new InvalidOperationException("No worksheet found in xlsx file.");
            var relationId = firstSheet.Attribute(XName.Get("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships"))?.Value
                ?? throw new InvalidOperationException("Worksheet relation not found.");

            var target = workbookRels.Descendants(relNs + "Relationship")
                .FirstOrDefault(r => string.Equals(r.Attribute("Id")?.Value, relationId, StringComparison.Ordinal))
                ?.Attribute("Target")?.Value
                ?? throw new InvalidOperationException("Worksheet target not found.");

            var sheetPath = target.StartsWith("/") ? target.TrimStart('/') : $"xl/{target.TrimStart('/')}";
            var sheetEntry = archive.GetEntry(sheetPath.Replace("\\", "/"))
                ?? throw new InvalidOperationException("Worksheet data not found.");

            var sharedStrings = new List<string>();
            if (sharedStringsEntry != null)
            {
                var sharedDoc = XDocument.Load(sharedStringsEntry.Open());
                var sharedNs = sharedDoc.Root?.Name.Namespace ?? XNamespace.None;
                sharedStrings = sharedDoc.Descendants(sharedNs + "si")
                    .Select(si => string.Concat(si.Descendants(sharedNs + "t").Select(t => t.Value)))
                    .ToList();
            }

            var sheetDoc = XDocument.Load(sheetEntry.Open());
            var sheetNs = sheetDoc.Root?.Name.Namespace ?? XNamespace.None;
            var rows = sheetDoc.Descendants(sheetNs + "row").ToList();
            if (rows.Count <= 1)
            {
                return new List<Dictionary<string, string>>();
            }

            var headers = ReadSheetRow(rows[0], sheetNs, sharedStrings).Select(NormalizeHeader).ToList();
            var result = new List<Dictionary<string, string>>();

            foreach (var row in rows.Skip(1))
            {
                var values = ReadSheetRow(row, sheetNs, sharedStrings);
                var dictionary = new Dictionary<string, string>
                {
                    ["__row"] = row.Attribute("r")?.Value ?? string.Empty
                };

                for (var i = 0; i < headers.Count; i++)
                {
                    dictionary[headers[i]] = i < values.Count ? values[i] : string.Empty;
                }

                result.Add(dictionary);
            }

            return result;
        }

        private static List<string> ReadSheetRow(XElement row, XNamespace ns, List<string> sharedStrings)
        {
            var values = new List<string>();
            var currentColumnIndex = 0;

            foreach (var cell in row.Elements(ns + "c"))
            {
                var cellReference = cell.Attribute("r")?.Value;
                var targetColumnIndex = GetColumnIndex(cellReference);

                while (currentColumnIndex < targetColumnIndex)
                {
                    values.Add(string.Empty);
                    currentColumnIndex++;
                }

                var cellType = cell.Attribute("t")?.Value;
                var rawValue = cell.Element(ns + "v")?.Value ?? string.Empty;

                if (cellType == "s" && int.TryParse(rawValue, out var sharedIndex) && sharedIndex >= 0 && sharedIndex < sharedStrings.Count)
                {
                    values.Add(sharedStrings[sharedIndex]);
                    currentColumnIndex++;
                    continue;
                }

                if (cellType == "inlineStr")
                {
                    values.Add(string.Concat(cell.Descendants(ns + "t").Select(t => t.Value)));
                    currentColumnIndex++;
                    continue;
                }

                values.Add(rawValue);
                currentColumnIndex++;
            }

            return values;
        }

        private static int GetColumnIndex(string? cellReference)
        {
            if (string.IsNullOrWhiteSpace(cellReference))
            {
                return 0;
            }

            var columnName = new string(cellReference
                .TakeWhile(char.IsLetter)
                .Select(char.ToUpperInvariant)
                .ToArray());

            if (string.IsNullOrWhiteSpace(columnName))
            {
                return 0;
            }

            var columnIndex = 0;
            foreach (var letter in columnName)
            {
                columnIndex = (columnIndex * 26) + (letter - 'A' + 1);
            }

            return Math.Max(0, columnIndex - 1);
        }

        private static string NormalizeHeader(string header)
        {
            return new string((header ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray());
        }

        private static byte[] BuildXlsxFile(string sheetName, IReadOnlyList<string> headers, IReadOnlyList<string[]> rows)
        {
            using var stream = new MemoryStream();
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                AddEntry(archive, "[Content_Types].xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                    "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                    "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                    "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                    "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                    "<Override PartName=\"/docProps/core.xml\" ContentType=\"application/vnd.openxmlformats-package.core-properties+xml\"/>" +
                    "<Override PartName=\"/docProps/app.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.extended-properties+xml\"/>" +
                    "</Types>");

                AddEntry(archive, "_rels/.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                    "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties\" Target=\"docProps/core.xml\"/>" +
                    "<Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties\" Target=\"docProps/app.xml\"/>" +
                    "</Relationships>");

                AddEntry(archive, "docProps/app.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\" xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">" +
                    "<Application>Codex</Application>" +
                    "</Properties>");

                AddEntry(archive, "docProps/core.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<cp:coreProperties xmlns:cp=\"http://schemas.openxmlformats.org/package/2006/metadata/core-properties\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:dcterms=\"http://purl.org/dc/terms/\" xmlns:dcmitype=\"http://purl.org/dc/dcmitype/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                    "<dc:title>Exam Score Import Template</dc:title>" +
                    "<dc:creator>Codex</dc:creator>" +
                    $"<dcterms:created xsi:type=\"dcterms:W3CDTF\">{DateTime.UtcNow:O}</dcterms:created>" +
                    "</cp:coreProperties>");

                AddEntry(archive, "xl/workbook.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                    "<sheets><sheet name=\"" + EscapeXml(sheetName) + "\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                    "</workbook>");

                AddEntry(archive, "xl/_rels/workbook.xml.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                    "</Relationships>");

                var allRows = new List<string[]> { headers.ToArray() };
                allRows.AddRange(rows);
                AddEntry(archive, "xl/worksheets/sheet1.xml", BuildWorksheetXml(allRows));
            }

            return stream.ToArray();
        }

        private static string BuildWorksheetXml(IReadOnlyList<string[]> rows)
        {
            var builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            builder.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                builder.Append($"<row r=\"{rowIndex + 1}\">");
                var row = rows[rowIndex];

                for (var colIndex = 0; colIndex < row.Length; colIndex++)
                {
                    var cellRef = $"{GetColumnName(colIndex + 1)}{rowIndex + 1}";
                    builder.Append($"<c r=\"{cellRef}\" t=\"inlineStr\"><is><t>{EscapeXml(row[colIndex] ?? string.Empty)}</t></is></c>");
                }

                builder.Append("</row>");
            }

            builder.Append("</sheetData></worksheet>");
            return builder.ToString();
        }

        private static void AddEntry(ZipArchive archive, string path, string content)
        {
            var entry = archive.CreateEntry(path, CompressionLevel.Fastest);
            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            writer.Write(content);
        }

        private static string EscapeXml(string value)
        {
            return SecurityElement.Escape(value) ?? string.Empty;
        }

        private static string GetColumnName(int index)
        {
            var columnName = string.Empty;
            while (index > 0)
            {
                var remainder = (index - 1) % 26;
                columnName = (char)(65 + remainder) + columnName;
                index = (index - 1) / 26;
            }

            return columnName;
        }

        private static string GetRequiredValue(Dictionary<string, string> row, string rowLabel, params string[] keys)
        {
            var value = GetValue(row, keys);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{rowLabel}: {keys[0]} is required.");
            }

            return value.Trim();
        }

        private static string? GetValue(Dictionary<string, string> row, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (row.TryGetValue(NormalizeHeader(key), out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return null;
        }

        private static double? GetOptionalScore(Dictionary<string, string> row, string rowLabel, params string[] keys)
        {
            var value = GetValue(row, keys);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var normalized = value.Replace(",", ".", StringComparison.Ordinal);
            if (double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            if (double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out parsed))
            {
                return parsed;
            }

            throw new InvalidOperationException($"{rowLabel}: {keys[0]} must be a valid number.");
        }

        private ExamResponseDto MapToDto(Exam exam, Address? address, ExamLevel? licenseType = null, Guid? centerId = null)
        {
            return new ExamResponseDto
            {
                Id = exam.Id,
                ExamBatchId = exam.ExamBatchId,
                CourseId = exam.CourseId,
                CenterId = centerId ?? Guid.Empty,
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

        private sealed class ScoreSelectionContext
        {
            public Course Course { get; set; } = default!;
            public Term Term { get; set; } = default!;
            public ExamBatch Batch { get; set; } = default!;
            public List<Exam> Exams { get; set; } = new();
            public List<Guid> ExamIds { get; set; } = new();
            public Dictionary<ExamType, Exam> ExamsByType { get; set; } = new();
            public bool HasSimulationExam { get; set; }
        }

        private sealed class ScoreEntryCandidate
        {
            public CourseRegistration CourseRegistration { get; set; } = default!;
            public ExamRegistration ExamRegistration { get; set; } = default!;
            public User Student { get; set; } = default!;
        }
    }
}
