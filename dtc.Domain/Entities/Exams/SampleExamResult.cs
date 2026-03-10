using System;

namespace dtc.Domain.Entities.Exams
{
    public class SampleExamResult : BaseEntity
    {
        public Guid SampleExamId { get; private set; }
        public Guid StudentId { get; private set; }
        public double TotalScore { get; private set; }
        public int DurationSeconds { get; private set; }
        public bool IsPassed { get; private set; }
        public string UserAnswersJson { get; private set; } = string.Empty;

        protected SampleExamResult() { }

        public SampleExamResult(
            Guid sampleExamId,
            Guid studentId,
            double totalScore,
            int durationSeconds,
            bool isPassed,
            string userAnswersJson,
            Guid? createdBy = null)
        {
            if (sampleExamId == Guid.Empty)
                throw new ArgumentException("SampleExamId is required");

            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId is required");

            Id = Guid.NewGuid();
            SampleExamId = sampleExamId;
            StudentId = studentId;
            TotalScore = totalScore;
            DurationSeconds = durationSeconds;
            IsPassed = isPassed;
            UserAnswersJson = userAnswersJson ?? "{}";

            SetCreated(createdBy);
        }
    }
}
