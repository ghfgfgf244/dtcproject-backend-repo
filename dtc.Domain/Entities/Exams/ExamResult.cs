namespace dtc.Domain.Entities.Exams
{
    public class ExamResult
    {
        public Guid Id { get; private set; }
        public Guid ExamId { get; private set; }
        public Guid StudentId { get; private set; }
        public int AttemptNo { get; private set; }
        public double Score { get; private set; }
        public bool IsPassed { get; private set; }

        protected ExamResult() { }

        public ExamResult(
            Guid examId,
            Guid studentId,
            int attemptNo,
            DateTime takenAt)
        {
            if (examId == Guid.Empty)
                throw new ArgumentException("ExamId is required");

            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId is required");

            if (attemptNo <= 0)
                throw new ArgumentException("AttemptNo must be greater than 0");

            Id = Guid.NewGuid();
            ExamId = examId;
            StudentId = studentId;
            AttemptNo = attemptNo;
        }

        // ================== BEHAVIORS ==================

        public void Grade(double score, int passScore)
        {
            if (score < 0)
                throw new ArgumentException("Score must be non-negative");

            Score = score;
            IsPassed = score >= passScore;
        }

    }
}
