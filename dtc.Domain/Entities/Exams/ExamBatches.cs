using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Exams
{
    public class ExamBatch : BaseEntity
    {
        public string BatchName { get; private set; } = default!;
        public DateTime RegistrationStartDate { get; private set; }
        public DateTime RegistrationEndDate { get; private set; }
        public DateTime ExamStartDate { get; private set; }
        public int CurrentCandidates { get; private set; }
        public int MaxCandidates { get; private set; }
        public ExamBatchStatus Status { get; private set; }

        protected ExamBatch() { }

        public ExamBatch(
            string batchName,
            DateTime registrationStartDate,
            DateTime registrationEndDate,
            DateTime examStartDate,
            int maxCandidates,
            Guid? createdBy = null)
        {
            SetBatchName(batchName);
            SetMaxCandidates(maxCandidates);
            CurrentCandidates = 0;
            SetDates(registrationStartDate, registrationEndDate, examStartDate);
            Status = ExamBatchStatus.Pending;

            Id = Guid.NewGuid();
            SetCreated(createdBy);
        }

        // =========================
        // BEHAVIORS
        // =========================

        public bool UpdateInfo(
            string? batchName,
            DateTime? regStart,
            DateTime? regEnd,
            DateTime? examStart,
            int? maxCandidates = null,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(batchName) && BatchName != batchName.Trim())
            {
                BatchName = batchName.Trim();
                changed = true;
            }

            if (regStart.HasValue || regEnd.HasValue || examStart.HasValue)
            {
                var newRegStart = regStart ?? RegistrationStartDate;
                var newRegEnd = regEnd ?? RegistrationEndDate;
                var newExamStart = examStart ?? ExamStartDate;

                SetDates(newRegStart, newRegEnd, newExamStart);
                changed = true;
            }

            if (maxCandidates.HasValue && maxCandidates.Value != MaxCandidates)
            {
                SetMaxCandidates(maxCandidates.Value);
                changed = true;
            }

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public void ChangeStatus(ExamBatchStatus newStatus, Guid? updatedBy = null)
        {
            if (Status == newStatus) return;

            // Example simple workflow validation
            if (Status == ExamBatchStatus.Completed || Status == ExamBatchStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status from Completed or Cancelled");

            Status = newStatus;
            SetUpdated(updatedBy);
        }

        public bool AddCandidate(Guid? updatedBy = null)
        {
            if (CurrentCandidates >= MaxCandidates)
                throw new InvalidOperationException("Exam batch is full");

            CurrentCandidates++;
            SetUpdated(updatedBy);
            return true;
        }

        public bool RemoveCandidate(Guid? updatedBy = null)
        {
            if (CurrentCandidates <= 0)
                return false;

            CurrentCandidates--;
            SetUpdated(updatedBy);
            return true;
        }

        // =========================
        // PRIVATE RULES
        // =========================

        private void SetBatchName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("BatchName is required");

            BatchName = name.Trim();
        }

        private void SetDates(DateTime regStart, DateTime regEnd, DateTime examStart)
        {
            if (regStart >= regEnd)
                throw new ArgumentException("RegistrationStartDate must be before RegistrationEndDate");

            if (regEnd >= examStart)
                throw new ArgumentException("RegistrationEndDate must be before ExamStartDate");

            RegistrationStartDate = regStart;
            RegistrationEndDate = regEnd;
            ExamStartDate = examStart;
        }

        private void SetMaxCandidates(int max)
        {
            if (max <= 0)
                throw new ArgumentException("MaxCandidates must be greater than 0");

            MaxCandidates = max;
        }
    }
}
