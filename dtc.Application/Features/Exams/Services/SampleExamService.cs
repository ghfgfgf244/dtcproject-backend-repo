using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace dtc.Application.Features.Exams.Services
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

            var newItems = request.Questions.Select(q => new { q.QuestionId, q.Order }).ToList();
            var newQuestionIds = newItems.Select(x => x.QuestionId).Distinct().ToList();
            var allQuestions = await _unitOfWork.Questions.FindAsync(q => newQuestionIds.Contains(q.Id));
            var foundQuestionIds = allQuestions.Select(q => q.Id).ToHashSet();

            foreach (var item in newItems)
            {
                if (!foundQuestionIds.Contains(item.QuestionId))
                    throw new Exception($"Question with ID {item.QuestionId} not found.");
            }

            var currentLinks = (await _unitOfWork.SampleExamQuestions.FindAsync(seq => seq.SampleExamId == id)).ToList();
            var currentByQId = currentLinks.ToDictionary(seq => seq.QuestionId);

            var newByQId = newItems.ToDictionary(x => x.QuestionId, x => x.Order);

            var toRemove = currentLinks.Where(seq => !newByQId.ContainsKey(seq.QuestionId)).ToList();
            if (toRemove.Count > 0)
                await _unitOfWork.SampleExamQuestions.RemoveRange(toRemove);

            foreach (var item in newItems)
            {
                if (currentByQId.TryGetValue(item.QuestionId, out var existing))
                {
                    if (existing.QuestionOrder != item.Order)
                    {
                        existing.ChangeOrder(item.Order);
                        await _unitOfWork.SampleExamQuestions.UpdateAsync(existing);
                    }
                }
                else
                {
                    var link = new SampleExamQuestion(id, item.QuestionId, item.Order);
                    await _unitOfWork.SampleExamQuestions.AddAsync(link);
                }
            }

            sampleExam.SyncTotalQuestions(newItems.Count);
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

            var links = await _unitOfWork.SampleExamQuestions.FindAsync(seq => seq.SampleExamId == id);
            var orderedLinks = links.OrderBy(seq => seq.QuestionOrder).ToList();

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
            foreach (var link in orderedLinks)
            {
                var qEntity = await _unitOfWork.Questions.GetByIdAsync(link.QuestionId);
                if (qEntity != null)
                {
                    questions.Add(new QuestionResponseDto
                    {
                        Id = qEntity.Id,
                        Order = link.QuestionOrder,
                        Category = qEntity.Category,
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
            return sampleExams.Select(MapToDto);
        }

        public async Task<SampleExamResponseDto> CreateSampleExamWithQuestionsAsync(CreateSampleExamWithQuestionsRequestDto request)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
            if (course == null) throw new Exception("Course not found");

            var questionIds = request.Questions.Select(q => q.QuestionId).Distinct().ToList();
            var foundQuestions = await _unitOfWork.Questions.FindAsync(q => questionIds.Contains(q.Id));
            if (foundQuestions.Count() != questionIds.Count)
                throw new Exception("One or more questions not found");

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var sampleExam = new SampleExam(
                    courseId: request.CourseId,
                    examNo: request.ExamNo,
                    level: request.Level,
                    durationMinutes: request.DurationMinutes,
                    passingScore: request.PassingScore
                );

                await _unitOfWork.SampleExams.AddAsync(sampleExam);
                await _unitOfWork.SaveChangesAsync(); // get ID

                foreach (var qReq in request.Questions)
                {
                    var link = new SampleExamQuestion(sampleExam.Id, qReq.QuestionId, qReq.Order);
                    await _unitOfWork.SampleExamQuestions.AddAsync(link);
                }

                sampleExam.SyncTotalQuestions(request.Questions.Count);
                await _unitOfWork.SampleExams.UpdateAsync(sampleExam);

                await _unitOfWork.SaveChangesAsync();
                return MapToDto(sampleExam);
            });
        }

        public async Task<SampleTestResultResponseDto> DoSampleTestAsync(Guid sampleExamId, Guid studentId, SubmitSampleTestRequestDto request)
        {
            var sampleExam = await _unitOfWork.SampleExams.GetByIdAsync(sampleExamId);
            if (sampleExam == null) throw new Exception("Sample exam not found");

            if (!sampleExam.IsActive)
                throw new Exception("Sample exam is currently inactive.");

            var links = await _unitOfWork.SampleExamQuestions.FindAsync(seq => seq.SampleExamId == sampleExamId);
            var orderedLinks = links.OrderBy(seq => seq.QuestionOrder).ToList();

            double score = 0;
            double scorePerQuestion = sampleExam.TotalQuestions > 0 ? 100.0 / sampleExam.TotalQuestions : 0;
            var correctAnswersDict = new Dictionary<int, string>();

            foreach (var link in orderedLinks)
            {
                var qEntity = await _unitOfWork.Questions.GetByIdAsync(link.QuestionId);
                if (qEntity != null)
                {
                    correctAnswersDict[qEntity.Id] = ((int)qEntity.CorrectAnswer).ToString();

                    if (request.Answers.TryGetValue(qEntity.Id, out var userAnswer))
                    {
                        var normalizedAnswer = NormalizeSubmittedAnswer(userAnswer);
                        if (normalizedAnswer == qEntity.CorrectAnswer)
                        {
                            score += scorePerQuestion;
                        }
                    }
                }
            }

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
            });
        }

        private static SampleExamResponseDto MapToDto(SampleExam sampleExam)
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

        private static AnswerOption? NormalizeSubmittedAnswer(string? value)
        {
            var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();

            if (int.TryParse(normalized, out var numericValue) && numericValue >= 1 && numericValue <= 4)
                return (AnswerOption)numericValue;

            return normalized switch
            {
                "A" => AnswerOption.A,
                "B" => AnswerOption.B,
                "C" => AnswerOption.C,
                "D" => AnswerOption.D,
                _ => null
            };
        }
    }
}
