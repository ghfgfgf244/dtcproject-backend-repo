using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Exams
{
    public class Exam : BaseEntity
    {
        public Guid ExamBatchId { get; private set; }
        public string ExamName { get; private set; } = default!;
        public DateTime ExamDate { get; private set; }
        public ExamType ExamType { get; private set; }
        public int DurationMinutes { get; private set; }
        
        // Custom domain fields from previous iteration, keeping them if needed by logic
        public int TotalScore { get; private set; }
        public int PassScore { get; private set; }
        public ExamStatus Status { get; private set; }

        protected Exam() { }

        public Exam(
            Guid examBatchId,
            string name,
            DateTime examDate,
            ExamType examType,
            int durationMinutes,
            int totalScore,
            int passScore,
            Guid? createdBy = null)
        {
            if (examBatchId == Guid.Empty)
                throw new ArgumentException("ExamBatchId is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("ExamName is required");

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            if (totalScore <= 0)
                throw new ArgumentException("TotalScore must be greater than 0");

            if (passScore <= 0 || passScore > totalScore)
                throw new ArgumentException("PassScore must be between 1 and TotalScore");

            Id = Guid.NewGuid();
            ExamBatchId = examBatchId;
            ExamName = name.Trim();
            ExamDate = examDate;
            ExamType = examType;
            DurationMinutes = durationMinutes;
            TotalScore = totalScore;
            PassScore = passScore;

            Status = ExamStatus.Draft;
            SetCreated(createdBy);
        }

        // ================== BEHAVIORS ==================

        public void Schedule(DateTime examDate, Guid? updatedBy = null)
        {
            if (Status != ExamStatus.Draft)
                throw new InvalidOperationException("Only draft exam can be scheduled");

            ExamDate = examDate;
            Status = ExamStatus.Scheduled;
            SetUpdated(updatedBy);
        }

        public void Finish(Guid? updatedBy = null)
        {
            if (Status != ExamStatus.Scheduled)
                throw new InvalidOperationException("Only scheduled exam can be finished");

            Status = ExamStatus.Finished;
            SetUpdated(updatedBy);
        }

        public void Cancel(Guid? updatedBy = null)
        {
            if (Status == ExamStatus.Finished)
                throw new InvalidOperationException("Finished exam cannot be cancelled");

            Status = ExamStatus.Cancelled;
            SetUpdated(updatedBy);
        }

        public void ChangeScoreRule(int totalScore, int passScore, Guid? updatedBy = null)
        {
            if (Status != ExamStatus.Draft)
                throw new InvalidOperationException("Score rule can only be changed in draft state");

            if (passScore <= 0 || passScore > totalScore)
                throw new ArgumentException("Invalid score rule");

            TotalScore = totalScore;
            PassScore = passScore;
            SetUpdated(updatedBy);
        }
        
        public bool UpdateInfo(
            string? name, 
            int? durationMinutes,
            ExamType? examType,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(name) && ExamName != name.Trim())
            {
                ExamName = name.Trim();
                changed = true;
            }

            if (durationMinutes.HasValue && DurationMinutes != durationMinutes.Value)
            {
                if (durationMinutes.Value <= 0)
                    throw new ArgumentException("Duration must be greater than 0");
                    
                DurationMinutes = durationMinutes.Value;
                changed = true;
            }

            if (examType.HasValue && ExamType != examType.Value)
            {
                ExamType = examType.Value;
                changed = true;
            }

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }
    }
}
