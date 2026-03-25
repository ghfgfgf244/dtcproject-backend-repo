using System;

namespace dtc.Domain.Entities.Exams
{
    /// <summary>Bảng trung gian SampleExam–Question (many-to-many). Lưu trong MongoDB.</summary>
    public class SampleExamQuestion
    {
        public Guid Id { get; private set; }
        public Guid SampleExamId { get; private set; }
        public int QuestionId { get; private set; }
        public int QuestionOrder { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected SampleExamQuestion() { }

        public SampleExamQuestion(Guid sampleExamId, int questionId, int order)
        {
            if (sampleExamId == Guid.Empty)
                throw new ArgumentException("SampleExamId is required");
            if (questionId <= 0)
                throw new ArgumentException("QuestionId is invalid");

            if (order <= 0)
                throw new ArgumentException("QuestionOrder must be greater than 0");

            Id = Guid.NewGuid();
            SampleExamId = sampleExamId;
            QuestionId = questionId;
            QuestionOrder = order;
            CreatedAt = DateTime.UtcNow;
        }

        public void ChangeOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new ArgumentException("QuestionOrder must be greater than 0");

            QuestionOrder = newOrder;
        }
    }

}
