using dtc.Application.DTOs.Exams;
using dtc.Application.Interfaces.Exams;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Exams
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamResponseDto> CreateExamAsync(CreateExamRequestDto request, Guid adminId)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(request.ExamBatchId);
            if (batch == null) throw new Exception("Exam batch not found");

            var exam = new Exam(
                examBatchId: request.ExamBatchId,
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

        public async Task<object> GetExamResultsAsync(Guid examId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null) throw new Exception("Exam not found");

            var results = await _unitOfWork.ExamResults.FindAsync(r => r.ExamId == examId);

            var report = new List<object>();
            foreach (var r in results)
            {
                var student = await _unitOfWork.Users.GetByIdAsync(r.StudentId);
                report.Add(new
                {
                    StudentId = r.StudentId,
                    StudentName = student?.FullName ?? "Unknown",
                    Score = r.Score,
                    IsPassed = r.IsPassed,
                    AttemptNo = r.AttemptNo
                });
            }

            return new
            {
                ExamId = exam.Id,
                ExamName = exam.ExamName,
                PassScore = exam.PassScore,
                TotalResults = results.Count(),
                PassedCount = results.Count(r => r.IsPassed),
                Results = report
            };
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
