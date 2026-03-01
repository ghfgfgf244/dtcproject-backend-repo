namespace dtc.Domain.Entities.Exams
{
    public class SampleExam
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; private set; }

        public int ExamNo { get; private set; }
        public ExamLevel Level { get; private set; }

        public int DurationMinutes { get; private set; }
        public int PassingScore { get; private set; }

        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }


        private readonly List<SampleExamQuestion> _questionIds = new();
        public IReadOnlyCollection<SampleExamQuestion> QuestionIds => _questionIds.AsReadOnly();

        public int TotalQuestions => _questionIds.Count;

        protected SampleExam() { }

        public SampleExam(
            Guid courseId,
            int examNo,
            ExamLevel level,
            int durationMinutes,
            int passingScore)
        {
            if (examNo <= 0)
                throw new ArgumentException("ExamNo must be greater than 0");

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            if (passingScore < 0)
                throw new ArgumentException("PassingScore invalid");

            Id = Guid.NewGuid();
            CourseId = courseId;
            ExamNo = examNo;
            Level = level;
            DurationMinutes = durationMinutes;
            PassingScore = passingScore;

            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddQuestion(int questionId)
        {
            if (_questionIds.Any(q => q.QuestionId == questionId))
                throw new InvalidOperationException("Question already exists in exam");

            var order = _questionIds.Count + 1;
            _questionIds.Add(new SampleExamQuestion(Id, questionId, order));
        }

        public void RemoveQuestion(int questionId)
        {
            var question = _questionIds.FirstOrDefault(q => q.QuestionId == questionId);
            if (question == null) return;

            _questionIds.Remove(question);

            int order = 1;
            foreach (var q in _questionIds.OrderBy(x => x.QuestionOrder))
            {
                q.ChangeOrder(order++);
            }
        }

        public void ChangeQuestionOrder(int questionId, int newOrder)
        {
            if (newOrder <= 0 || newOrder > _questionIds.Count)
                throw new ArgumentException("Invalid order");

            var target = _questionIds.FirstOrDefault(q => q.QuestionId == questionId);
            if (target is null)
                throw new ArgumentException("Question not found");

            var oldOrder = target.QuestionOrder;
            if (oldOrder == newOrder)
                return;

            var swapped = _questionIds.First(q => q.QuestionOrder == newOrder);
            swapped.ChangeOrder(oldOrder);

            target.ChangeOrder(newOrder);
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

    }
}
