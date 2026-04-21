using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.AI.Interfaces;
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
        private readonly IAiRouterService _aiRouterService;

        public SampleExamService(IUnitOfWork unitOfWork, IAiRouterService aiRouterService)
        {
            _unitOfWork = unitOfWork;
            _aiRouterService = aiRouterService;
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

        public async Task<PublicSampleExamDetailResponseDto> GetPublicSampleExamDetailAsync(Guid id)
        {
            var sampleExam = await _unitOfWork.SampleExams.GetByIdAsync(id);
            if (sampleExam == null) throw new Exception("Sample exam not found");

            var links = await _unitOfWork.SampleExamQuestions.FindAsync(seq => seq.SampleExamId == id);
            var orderedLinks = links.OrderBy(seq => seq.QuestionOrder).ToList();

            var dto = new PublicSampleExamDetailResponseDto
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

            var questions = new List<PublicQuestionResponseDto>();
            foreach (var link in orderedLinks)
            {
                var qEntity = await _unitOfWork.Questions.GetByIdAsync(link.QuestionId);
                if (qEntity != null)
                {
                    questions.Add(new PublicQuestionResponseDto
                    {
                        Id = qEntity.Id,
                        Order = link.QuestionOrder,
                        Category = qEntity.Category,
                        Content = qEntity.Content,
                        AnswerA = qEntity.AnswerA,
                        AnswerB = qEntity.AnswerB,
                        AnswerC = qEntity.AnswerC,
                        AnswerD = qEntity.AnswerD,
                        ImageLink = qEntity.ImageLink
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

        public Task<SampleTestResultResponseDto> DoSampleTestAsync(Guid sampleExamId, Guid studentId, SubmitSampleTestRequestDto request)
        {
            return EvaluateSampleTestAsync(sampleExamId, request, studentId, persistResult: true);
        }

        public Task<SampleTestResultResponseDto> DoPublicSampleTestAsync(Guid sampleExamId, SubmitSampleTestRequestDto request)
        {
            return EvaluateSampleTestAsync(sampleExamId, request, null, persistResult: false);
        }

        private async Task<SampleTestResultResponseDto> EvaluateSampleTestAsync(
            Guid sampleExamId,
            SubmitSampleTestRequestDto request,
            Guid? studentId,
            bool persistResult)
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
            var explanationsDict = new Dictionary<int, string>();
            var reviewItems = new List<SampleExamQuestionReviewDto>();
            var wrongCountsByCategory = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var suggestedTopics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var questionsToUpdate = new List<Question>();
            var correctCount = 0;

            foreach (var link in orderedLinks)
            {
                var qEntity = await _unitOfWork.Questions.GetByIdAsync(link.QuestionId);
                if (qEntity != null)
                {
                    var correctAnswer = ((int)qEntity.CorrectAnswer).ToString();
                    correctAnswersDict[qEntity.Id] = correctAnswer;
                    if (!string.IsNullOrWhiteSpace(qEntity.Explanation))
                    {
                        explanationsDict[qEntity.Id] = qEntity.Explanation!;
                    }

                    request.Answers.TryGetValue(qEntity.Id, out var userAnswer);
                    var normalizedAnswer = NormalizeSubmittedAnswer(userAnswer);
                    var isCorrect = normalizedAnswer == qEntity.CorrectAnswer;

                    qEntity.RegisterAttempt(isCorrect);
                    questionsToUpdate.Add(qEntity);

                    if (isCorrect)
                    {
                        score += scorePerQuestion;
                        correctCount++;
                    }
                    else
                    {
                        wrongCountsByCategory[qEntity.Category] = wrongCountsByCategory.TryGetValue(qEntity.Category, out var currentCount)
                            ? currentCount + 1
                            : 1;

                        suggestedTopics.Add(GetSuggestedTopic(qEntity.Category));
                    }

                    reviewItems.Add(new SampleExamQuestionReviewDto
                    {
                        QuestionId = qEntity.Id,
                        Category = qEntity.Category,
                        IsCorrect = isCorrect,
                        SelectedAnswer = normalizedAnswer.HasValue ? ((int)normalizedAnswer.Value).ToString() : null,
                        CorrectAnswer = correctAnswer,
                        Explanation = qEntity.Explanation,
                        StudyTip = BuildStudyTip(qEntity.Category, isCorrect),
                        AttemptCount = qEntity.AttemptCount,
                        WrongAttemptCount = qEntity.WrongAttemptCount,
                        WrongRate = qEntity.WrongRate
                    });
                }
            }

            score = Math.Round(score, 2);
            bool isPassed = score >= sampleExam.PassingScore;

            var answersJson = JsonSerializer.Serialize(request.Answers);

            var insight = await BuildMockExamInsightAsync(
                sampleExam.Level,
                score,
                sampleExam.PassingScore,
                reviewItems,
                wrongCountsByCategory,
                suggestedTopics);

            Guid resultId = Guid.Empty;

            if (persistResult && studentId.HasValue)
            {
                var result = new SampleExamResult(
                    sampleExamId: sampleExamId,
                    studentId: studentId.Value,
                    totalScore: score,
                    durationSeconds: request.DurationSeconds,
                    isPassed: isPassed,
                    userAnswersJson: answersJson,
                    createdBy: studentId.Value
                );

                await _unitOfWork.SampleExamResults.AddAsync(result);
                await _unitOfWork.SaveChangesAsync();
                resultId = result.Id;
            }

            foreach (var question in questionsToUpdate)
            {
                await _unitOfWork.Questions.UpdateAsync(question);
            }

            var wrongCount = Math.Max(reviewItems.Count - correctCount, 0);

            return new SampleTestResultResponseDto
            {
                ResultId = resultId,
                TotalScore = score,
                DurationSeconds = request.DurationSeconds,
                IsPassed = isPassed,
                TotalQuestions = reviewItems.Count,
                CorrectCount = correctCount,
                WrongCount = wrongCount,
                CorrectAnswers = correctAnswersDict,
                Explanations = explanationsDict,
                ReviewItems = reviewItems,
                Insight = insight
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

        private async Task<SampleExamInsightDto> BuildMockExamInsightAsync(
            ExamLevel level,
            double score,
            int passingScore,
            IReadOnlyCollection<SampleExamQuestionReviewDto> reviewItems,
            IReadOnlyDictionary<string, int> wrongCountsByCategory,
            IReadOnlyCollection<string> suggestedTopics)
        {
            var fallbackSummary = BuildFallbackInsight(score, passingScore, wrongCountsByCategory, suggestedTopics);

            if (reviewItems.Count == 0)
            {
                return new SampleExamInsightDto
                {
                    Summary = fallbackSummary,
                    Model = "rule-based",
                    WrongCountsByCategory = new Dictionary<string, int>(wrongCountsByCategory),
                    SuggestedTopics = suggestedTopics.ToList()
                };
            }

            try
            {
                var wrongQuestionLines = reviewItems
                    .Where(item => !item.IsCorrect)
                    .Take(8)
                    .Select(item => $"- Cau {item.QuestionId} | Nhom: {item.Category} | Ty le sai he thong: {item.WrongRate:P0}");

                var prompt =
                    "Ban la tro ly on tap ly thuyet lai xe. " +
                    "Hay viet mot doan nhan xet ngan gon bang tieng Viet, than thien, tap trung vao nhom kien thuc can on lai. " +
                    $"Hang GPLX: {level}. " +
                    $"Diem bai thi: {score}/100. Muc dat: {passingScore}/100. " +
                    $"So cau sai theo nhom: {string.Join(", ", wrongCountsByCategory.Select(item => $"{item.Key}={item.Value}"))}. " +
                    $"Chu de on tap goi y: {string.Join(", ", suggestedTopics)}. " +
                    "Danh sach cau sai tieu bieu: " +
                    string.Join(" ", wrongQuestionLines) +
                    " Hay tra ve 3-4 cau, khong dung markdown.";

                var aiResult = await _aiRouterService.GenerateAsync("mock-exam-insight", prompt);

                return new SampleExamInsightDto
                {
                    Summary = string.IsNullOrWhiteSpace(aiResult.Content) ? fallbackSummary : aiResult.Content.Trim(),
                    Model = string.IsNullOrWhiteSpace(aiResult.Model) ? "rule-based" : aiResult.Model,
                    WrongCountsByCategory = new Dictionary<string, int>(wrongCountsByCategory),
                    SuggestedTopics = suggestedTopics.ToList()
                };
            }
            catch
            {
                return new SampleExamInsightDto
                {
                    Summary = fallbackSummary,
                    Model = "rule-based",
                    WrongCountsByCategory = new Dictionary<string, int>(wrongCountsByCategory),
                    SuggestedTopics = suggestedTopics.ToList()
                };
            }
        }

        private static string BuildFallbackInsight(
            double score,
            int passingScore,
            IReadOnlyDictionary<string, int> wrongCountsByCategory,
            IReadOnlyCollection<string> suggestedTopics)
        {
            var dominantCategory = wrongCountsByCategory
                .OrderByDescending(item => item.Value)
                .Select(item => item.Key)
                .FirstOrDefault();

            if (wrongCountsByCategory.Count == 0)
            {
                return score >= passingScore
                    ? "Bạn đang nắm bài khá tốt. Hãy tiếp tục giữ nhịp ôn tập để duy trì tốc độ làm bài và độ chính xác."
                    : "Bạn chưa có nhiều câu sai nổi bật theo nhóm, nhưng điểm số vẫn chưa đạt. Nên luyện thêm để tăng tốc độ và độ ổn định khi làm bài.";
            }

            var topicText = suggestedTopics.Count > 0
                ? $" Ưu tiên ôn lại: {string.Join(", ", suggestedTopics)}."
                : string.Empty;

            return $"Bạn đang mất điểm nhiều nhất ở nhóm {dominantCategory}.{topicText} Tập trung làm lại các câu đã sai và đọc kỹ giải thích sau mỗi lần nộp bài.";
        }

        private static string BuildStudyTip(string category, bool isCorrect)
        {
            if (isCorrect)
            {
                return "Bạn đã trả lời đúng câu này. Nên ghi nhớ quy tắc và tiếp tục giữ nhịp làm bài ổn định.";
            }

            return QuestionCategoryNames.Normalize(category) switch
            {
                QuestionCategoryNames.Sign => "Nên học lại theo nhóm biển báo và liên kết ý nghĩa biển với tình huống giao thông cụ thể.",
                QuestionCategoryNames.Simulation => "Nên quan sát hướng đi, điểm xung đột và thứ tự ưu tiên của từng xe trong tình huống sa hình.",
                _ => "Nên đọc kỹ mẹo ghi nhớ, đối chiếu đáp án đúng và làm lại nhóm câu lý thuyết tương tự."
            };
        }

        private static string GetSuggestedTopic(string category)
        {
            return QuestionCategoryNames.Normalize(category) switch
            {
                QuestionCategoryNames.Sign => "Nhận diện và ý nghĩa biển báo",
                QuestionCategoryNames.Simulation => "Xử lý tình huống sa hình",
                _ => "Quy tắc và khái niệm lý thuyết"
            };
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
