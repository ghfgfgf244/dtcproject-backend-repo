namespace dtc.Domain.Entities.Exams
{
    public class Question
    {
        public int Id { get; private set; }
        public string Category { get; private set; } = QuestionCategoryNames.Theory;
        public string Content { get; private set; } = string.Empty;
        public string? AnswerA { get; private set; }
        public string? AnswerB { get; private set; }
        public string? AnswerC { get; private set; }
        public string? AnswerD { get; private set; }
        public AnswerOption CorrectAnswer { get; private set; }
        public string? ImageLink { get; private set; }
        public string? Explanation { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Question() { }

        public Question(
            string category,
            string content,
            AnswerOption correctAnswer,
            string? a = null,
            string? b = null,
            string? c = null,
            string? d = null,
            string? imageLink = null,
            string? explanation = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Question content is required");

            ValidateAnswers(correctAnswer, a, b, c, d);

            Category = QuestionCategoryNames.Normalize(category);
            Content = content.Trim();
            CorrectAnswer = correctAnswer;
            AnswerA = a;
            AnswerB = b;
            AnswerC = c;
            AnswerD = d;
            ImageLink = imageLink;
            Explanation = explanation;
            CreatedAt = DateTime.UtcNow;
        }

        // ================== BEHAVIORS ==================

        public void AssignIdentity(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Question ID must be greater than zero.");

            Id = id;
        }

        public void UpdateContent(
            string category,
            string content,
            string? a,
            string? b,
            string? c,
            string? d,
            AnswerOption correctAnswer,
            string? imageLink = null,
            string? explanation = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Question content is required");

            ValidateAnswers(correctAnswer, a, b, c, d);

            Category = QuestionCategoryNames.Normalize(category);
            Content = content.Trim();
            AnswerA = a;
            AnswerB = b;
            AnswerC = c;
            AnswerD = d;
            CorrectAnswer = correctAnswer;
            ImageLink = imageLink;
            Explanation = explanation;
        }

        // ================== DOMAIN VALIDATION ==================

        private static void ValidateAnswers(
            AnswerOption correct,
            string? a,
            string? b,
            string? c,
            string? d)
        {
            var answers = new Dictionary<AnswerOption, string?>
            {
                { AnswerOption.A, a },
                { AnswerOption.B, b },
                { AnswerOption.C, c },
                { AnswerOption.D, d }
            };

            if (!answers.TryGetValue(correct, out var value) ||
                string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Correct answer must exist");
            }
        }
    }
}
