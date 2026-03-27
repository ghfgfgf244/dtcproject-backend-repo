using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Email.Interfaces;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            if (batch == null) throw new Exception("Exam batch not found");

            var exam = new Exam(
                examBatchId: request.ExamBatchId,
                courseId: request.CourseId,
                name: request.ExamName,
                examDate: request.ExamDate,
                examType: request.ExamType,
                durationMinutes: request.DurationMinutes,
                totalScore: request.TotalScore,
                passScore: request.PassScore,
                createdBy: adminId
            );

            await _unitOfWork.Exams.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            // Side-effect: notify students registered for this exam batch
            _ = Task.Run(async () =>
            {
                try
                {
                    var registrations = await _unitOfWork.ExamRegistrations.FindAsync(
                        r => r.ExamBatchId == request.ExamBatchId && r.Status == ExamRegistrationStatus.Approved);

                    foreach (var reg in registrations)
                    {
                        await _notificationService.CreateForUserAsync(
                            reg.StudentId,
                            "Lịch thi mới",
                            $"Kỳ thi '{exam.ExamName}' sẽ diễn ra vào ngày {exam.ExamDate:dd/MM/yyyy}.",
                            NotificationType.Exam);
                        await Task.Delay(50); // throttle nhẹ giữa mỗi notify
                    }
                }
                catch { /* không làm hỏng luồng chính */ }
            });

            return MapToDto(exam);
        }

        public async Task<ExamResponseDto> UpdateExamAsync(Guid id, UpdateExamRequestDto request, Guid adminId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new Exception("Exam not found");

            exam.UpdateInfo(
                name: request.ExamName,
                durationMinutes: request.DurationMinutes,
                examType: request.ExamType,
                updatedBy: adminId
            );

            if (request.ExamDate.HasValue)
            {
                exam.Schedule(request.ExamDate.Value, adminId);
            }

            if (request.TotalScore.HasValue && request.PassScore.HasValue)
            {
                exam.ChangeScoreRule(request.TotalScore.Value, request.PassScore.Value, adminId);
            }

            if (request.Status.HasValue)
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

            return MapToDto(exam);
        }

        public async Task<bool> DeleteExamAsync(Guid id, Guid adminId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new Exception("Exam not found");

            exam.SoftDelete(adminId);
            await _unitOfWork.Exams.UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ExamResponseDto> GetExamDetailAsync(Guid id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new Exception("Exam not found");

            return MapToDto(exam);
        }

        public async Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync()
        {
            var exams = await _unitOfWork.Exams.GetAllAsync();
            return exams.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<ExamResponseDto>> GetExamsByCourseAsync(Guid courseId)
        {
            var exams = await _unitOfWork.Exams.FindAsync(e => e.CourseId == courseId);
            return exams.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<ExamResponseDto>> GetExamsByBatchAsync(Guid batchId)
        {
            var exams = await _unitOfWork.Exams.FindAsync(e => e.ExamBatchId == batchId);
            return exams.Select(MapToDto).ToList();
        }

        public async Task<object> GetExamResultsAsync(Guid examId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null) throw new Exception("Exam not found");

            var batchId = exam.ExamBatchId;
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(batchId);
            
            // Lấy tất cả các kỳ thi trong đợt này (Theory, Simulation, Practice)
            var allExamsInBatch = await _unitOfWork.Exams.FindAsync(e => e.ExamBatchId == batchId);
            var examIds = allExamsInBatch.Select(e => e.Id).ToList();
            var examTypeMap = allExamsInBatch.ToDictionary(e => e.Id, e => e.ExamType);

            // Lấy tất cả kết quả của các kỳ thi này
            var allResults = await _unitOfWork.ExamResults.FindAsync(r => examIds.Contains(r.ExamId));
            
            // Lấy thông tin học viên
            var studentIds = allResults.Select(r => r.StudentId).Distinct().ToList();
            var students = await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id));
            var studentMap = students.ToDictionary(u => u.Id);

            // Nhóm theo học viên và lấy điểm cao nhất cho mỗi loại hình thi
            var studentReports = allResults
                .GroupBy(r => r.StudentId)
                .Select(g => 
                {
                    var studentId = g.Key;
                    var student = studentMap.GetValueOrDefault(studentId);
                    var studentResults = g.ToList();

                    // Lấy kết quả tốt nhất (điểm cao nhất) cho mỗi loại
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
                        //CenterId = student?.CenterId,
                        
                        // Kết quả Lý thuyết
                        TheoryScore = theoryResult?.Score,
                        TheoryPassed = theoryResult?.IsPassed ?? false,
                        
                        // Kết quả Mô phỏng
                        SimulationScore = simulationResult?.Score,
                        SimulationPassed = simulationResult?.IsPassed ?? false,
                        
                        // Kết quả Thực hành (Xa trường)
                        PracticeScore = practiceResult?.Score,
                        PracticePassed = practiceResult?.IsPassed ?? false,

                        IsFullyQualified = (theoryResult?.IsPassed == true) && 
                                          (simulationResult?.IsPassed == true) && 
                                          (practiceResult?.IsPassed == true)
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
            if (result == null) throw new Exception("Exam result not found");

            var exam = await _unitOfWork.Exams.GetByIdAsync(result.ExamId);
            if (exam == null) throw new Exception("Exam not found");

            // Update score
            result.Grade(request.Score, exam.PassScore);
            
            await _unitOfWork.ExamResults.UpdateAsync(result);
            await _unitOfWork.SaveChangesAsync();

            // Side-effect: notify + email student about result
            _ = Task.Run(async () => {
                await NotifyStudentResultAsync(result, exam);
            });

            return true;
        }

        public async Task<bool> EnterBulkExamResultsAsync(BulkExamResultRequestDto request, Guid adminId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(request.ExamId);
            if (exam == null) throw new Exception("Exam not found");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var studentIds = request.Results.Select(r => r.StudentId).Distinct().ToList();
                var students = await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id));
                if (students.Count() != studentIds.Count)
                    throw new Exception("One or more students not found");

                var existingResults = await _unitOfWork.ExamResults.FindAsync(r => r.ExamId == request.ExamId && studentIds.Contains(r.StudentId));
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

                // Side-effect: Notifications
                _ = Task.Run(async () =>
                {
                    foreach (var res in processedResults)
                    {
                        await NotifyStudentResultAsync(res, exam);
                    }
                });

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task NotifyStudentResultAsync(ExamResult result, Exam exam)
        {
            try
            {
                var student = await _unitOfWork.Users.GetByIdAsync(result.StudentId);
                if (student != null)
                {
                    var statusText = result.IsPassed ? "ĐẠT" : "KHÔNG ĐẠT";
                    await _notificationService.CreateForUserAsync(
                        result.StudentId,
                        "Kết quả thi",
                        $"Kỳ thi '{exam.ExamName}': điểm {result.Score:0.##} — {statusText}.",
                        NotificationType.ExamResult);

                    await _emailService.SendExamResultAsync(
                        student.Email.Value,
                        student.FullName,
                        exam.ExamName,
                        result.Score,
                        result.IsPassed);
                }
            }
            catch { }
        }

        private ExamResponseDto MapToDto(Exam exam)
        {
            return new ExamResponseDto
            {
                Id = exam.Id,
                ExamBatchId = exam.ExamBatchId,
                ExamName = exam.ExamName,
                ExamDate = exam.ExamDate,
                ExamType = exam.ExamType,
                DurationMinutes = exam.DurationMinutes,
                TotalScore = exam.TotalScore,
                PassScore = exam.PassScore,
                Status = exam.Status,
                CreatedAt = exam.CreatedAt
            };
        }
    }
}
