namespace dtc.Domain.Entities.Exams
{
    public class SampleExamQuestion
    {
        public Guid SampleExamId { get; private set; }
        public int QuestionId { get; private set; }
        public int QuestionOrder { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected SampleExamQuestion() { }

        internal SampleExamQuestion(Guid sampleExamId, int questionId, int order)
        {
            if (questionId <= 0)
                throw new ArgumentException("QuestionId is invalid");

            if (order <= 0)
                throw new ArgumentException("QuestionOrder must be greater than 0");

            SampleExamId = sampleExamId;
            QuestionId = questionId;
            QuestionOrder = order;
            CreatedAt = DateTime.UtcNow;
        }

        internal void ChangeOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new ArgumentException("QuestionOrder must be greater than 0");

            QuestionOrder = newOrder;
        }
    }

}
