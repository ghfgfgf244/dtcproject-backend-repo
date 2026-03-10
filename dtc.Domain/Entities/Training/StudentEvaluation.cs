using System;

namespace dtc.Domain.Entities.Training
{
    public class StudentEvaluation : BaseEntity
    {
        public Guid StudentId { get; private set; }
        public Guid InstructorId { get; private set; }
        public Guid? ClassId { get; private set; }
        public int PunctualityScore { get; private set; }
        public int SkillLevel { get; private set; }
        public string Note { get; private set; }
        public DateTime EvaluationDate { get; private set; }

        protected StudentEvaluation() { }

        public StudentEvaluation(
            Guid studentId,
            Guid instructorId,
            Guid? classId,
            int punctualityScore,
            int skillLevel,
            string note,
            Guid? createdBy = null)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId is required");

            if (instructorId == Guid.Empty)
                throw new ArgumentException("InstructorId is required");

            Id = Guid.NewGuid();
            StudentId = studentId;
            InstructorId = instructorId;
            ClassId = classId;
            PunctualityScore = EnsureValidScore(punctualityScore);
            SkillLevel = EnsureValidScore(skillLevel);
            Note = note ?? string.Empty;
            EvaluationDate = DateTime.UtcNow;

            SetCreated(createdBy);
        }

        public void UpdateEvaluation(int punctualityScore, int skillLevel, string note, Guid? updatedBy = null)
        {
            PunctualityScore = EnsureValidScore(punctualityScore);
            SkillLevel = EnsureValidScore(skillLevel);
            if (note != null)
                Note = note;

            SetUpdated(updatedBy);
        }

        private int EnsureValidScore(int score)
        {
            if (score < 1) return 1;
            if (score > 10) return 10;
            return score;
        }
    }
}
