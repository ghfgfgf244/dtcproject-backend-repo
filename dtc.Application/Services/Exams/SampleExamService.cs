using dtc.Application.DTOs.Exams;
using dtc.Application.Interfaces.Exams;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Exams
{
    public class SampleExamService : ISampleExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SampleExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SampleExamResponseDto> CreateSampleExamAsync(CreateSampleExamRequestDto request)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null) throw new Exception("Course not found");

            var sampleExam = new SampleExam(
                courseId: request.CourseId,
                examNo: request.ExamNo,
                level: request.Level,
                durationMinutes: request.DurationMinutes,
                passingScore: request.PassingScore
            );

            await _unitOfWork.SampleExams.AddAsync(sampleExam);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(sampleExam);
        }

        public async Task<SampleExamResponseDto> UpdateSampleExamQuestionsAsync(Guid id, UpdateSampleExamQuestionsRequestDto request)
        {
            var sampleExam = await _unitOfWork.SampleExams.GetByIdAsync(id);
            if (sampleExam == null) throw new Exception("Sample exam not found");

            // Verify questions exist
            var newQuestionIds = request.Questions.Select(q => q.QuestionId).ToList();
            var allQuestions = await _unitOfWork.Questions.FindAsync(q => newQuestionIds.Contains(q.Id));
            var foundQuestionIds = allQuestions.Select(q => q.Id).ToHashSet();

            foreach (var requestedQ in request.Questions)
            {
                if (!foundQuestionIds.Contains(requestedQ.QuestionId))
                    throw new Exception($"Question with ID {requestedQ.QuestionId} not found.");
            }

            // Sync questions (delete removed, add new, change order)
            var currentLinks = sampleExam.QuestionIds.ToList();
            
            // Remove missing
            foreach (var link in currentLinks)
            {
                if (!newQuestionIds.Contains(link.QuestionId))
                {
                    sampleExam.RemoveQuestion(link.QuestionId);
                }
            }

            // Add or reorder
            foreach (var requestedQ in request.Questions)
            {
                if (!currentLinks.Any(c => c.QuestionId == requestedQ.QuestionId))
                {
                    sampleExam.AddQuestion(requestedQ.QuestionId);
                    sampleExam.ChangeQuestionOrder(requestedQ.QuestionId, requestedQ.Order);
                }
                else
                {
                    sampleExam.ChangeQuestionOrder(requestedQ.QuestionId, requestedQ.Order);
                }
            }

            await _unitOfWork.SampleExams.UpdateAsync(sampleExam);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(sampleExam);
        }

        public async Task<bool> DeleteSampleExamAsync(Guid id)
        {
            var sampleExam = await _unitOfWork.SampleExams.GetByIdAsync(id);
            if (sampleExam == null) throw new Exception("Sample exam not found");

            sampleExam.Deactivate();

            await _unitOfWork.SampleExams.UpdateAsync(sampleExam);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<SampleExamDetailResponseDto> GetSampleExamDetailAsync(Guid id)
        {
            var sampleExam = await _unitOfWork.SampleExams.GetByIdAsync(id);
            if (sampleExam == null) throw new Exception("Sample exam not found");

            var dto = new SampleExamDetailResponseDto
            {
                Id = sampleExam.Id,
                CourseId = sampleExam.CourseId,
                ExamNo = sampleExam.ExamNo,
                Level = sampleExam.Level,
                DurationMinutes = sampleExam.DurationMinutes,
                PassingScore = sampleExam.PassingScore,
                IsActive = sampleExam.IsActive,
                CreatedAt = sampleExam.CreatedAt,
                TotalQuestions = sampleExam.TotalQuestions
            };

            var questions = new List<QuestionResponseDto>();
            foreach (var link in sampleExam.QuestionIds.OrderBy(q => q.QuestionOrder))
            {
                var qEntity = await _unitOfWork.Questions.GetByIdAsync(link.QuestionId);
                if (qEntity != null)
                {
                    questions.Add(new QuestionResponseDto
                    {
                        Id = qEntity.Id,
                        Content = qEntity.Content,
                        AnswerA = qEntity.AnswerA,
                        AnswerB = qEntity.AnswerB,
                        AnswerC = qEntity.AnswerC,
                        AnswerD = qEntity.AnswerD,
                        CorrectAnswer = qEntity.CorrectAnswer,
                        ImageLink = qEntity.ImageLink,
                        Explanation = qEntity.Explanation,
                        CreatedAt = qEntity.CreatedAt
                    });
                }
            }

            dto.Questions = questions;
            return dto;
        }

        public async Task<IEnumerable<SampleExamResponseDto>> GetAllSampleExamsAsync()
        {
            var sampleExams = await _unitOfWork.SampleExams.GetAllAsync();
            var dtos = new List<SampleExamResponseDto>();
            foreach (var se in sampleExams)
            {
                dtos.Add(MapToDto(se));
            }
            return dtos;
        }

        public async Task<SampleTestResultResponseDto> DoSampleTestAsync(Guid sampleExamId, Guid studentId, SubmitSampleTestRequestDto request)
        {
            var sampleExam = await _unitOfWork.SampleExams.GetByIdAsync(sampleExamId);
            if (sampleExam == null) throw new Exception("Sample exam not found");

            if (!sampleExam.IsActive)
                throw new Exception("Sample exam is currently inactive.");

            double score = 0;
            double scorePerQuestion = sampleExam.TotalQuestions > 0 ? 100.0 / sampleExam.TotalQuestions : 0;
            var correctAnswersDict = new Dictionary<int, string>();

            foreach (var link in sampleExam.QuestionIds)
            {
                var qEntity = await _unitOfWork.Questions.GetByIdAsync(link.QuestionId);
                if (qEntity != null)
                {
                    correctAnswersDict[qEntity.Id] = qEntity.CorrectAnswer.ToString();

                    if (request.Answers.TryGetValue(qEntity.Id, out var userAnswer))
                    {
                        if (userAnswer.ToUpper() == qEntity.CorrectAnswer.ToString())
                        {
                            score += scorePerQuestion;
                        }
                    }
                }
            }

            // Prevent floating point precision weirdness causing a fail if it's 99.9999 vs 100
            score = Math.Round(score, 2);
            bool isPassed = score >= sampleExam.PassingScore;

            var answersJson = JsonSerializer.Serialize(request.Answers);

            var result = new SampleExamResult(
                sampleExamId: sampleExamId,
                studentId: studentId,
                totalScore: score,
                durationSeconds: request.DurationSeconds,
                isPassed: isPassed,
                userAnswersJson: answersJson,
                createdBy: studentId
            );

            await _unitOfWork.SampleExamResults.AddAsync(result);
            await _unitOfWork.SaveChangesAsync();

            return new SampleTestResultResponseDto
            {
                ResultId = result.Id,
                TotalScore = score,
                DurationSeconds = request.DurationSeconds,
                IsPassed = isPassed,
                CorrectAnswers = correctAnswersDict
            };
        }

        public async Task<IEnumerable<SampleTestResultResponseDto>> GetSampleTestResultsForStudentAsync(Guid studentId)
        {
            var results = await _unitOfWork.SampleExamResults.FindAsync(r => r.StudentId == studentId);
            return results.Select(r => new SampleTestResultResponseDto
            {
                ResultId = r.Id,
                TotalScore = r.TotalScore,
                DurationSeconds = r.DurationSeconds,
                IsPassed = r.IsPassed
            }).ToList();
        }

        private SampleExamResponseDto MapToDto(SampleExam sampleExam)
        {
            return new SampleExamResponseDto
            {
                Id = sampleExam.Id,
                CourseId = sampleExam.CourseId,
                ExamNo = sampleExam.ExamNo,
                Level = sampleExam.Level,
                DurationMinutes = sampleExam.DurationMinutes,
                PassingScore = sampleExam.PassingScore,
                IsActive = sampleExam.IsActive,
                CreatedAt = sampleExam.CreatedAt,
                TotalQuestions = sampleExam.TotalQuestions
            };
        }
    }
}
