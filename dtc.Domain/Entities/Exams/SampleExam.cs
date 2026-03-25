using System;

namespace dtc.Domain.Entities.Exams
{
    /// <summary>
    /// Đề thi mẫu. Liên kết với câu hỏi qua bảng trung gian <see cref="SampleExamQuestion"/>.
    /// Không lưu danh sách câu hỏi trong entity.
    /// </summary>
    public class SampleExam
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; private set; }

        public int ExamNo { get; private set; }
        public ExamLevel Level { get; private set; }

        public int DurationMinutes { get; private set; }
        public int PassingScore { get; private set; }

        /// <summary>Sĩ số câu hỏi (đồng bộ từ SampleExamQuestion bởi Application layer).</summary>
        public int TotalQuestions { get; private set; }

        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

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
            TotalQuestions = 0;

            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>Đồng bộ sĩ số từ SampleExamQuestion (gọi từ Application layer).</summary>
        public void SyncTotalQuestions(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            TotalQuestions = count;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
