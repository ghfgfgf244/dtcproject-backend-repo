using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dtc.Domain.Entities.Location;

namespace dtc.Domain.Entities.Exams
{
    public class ExamBatch : BaseEntity
    {
        public ExamBatchScopeType ScopeType { get; private set; }
        public Guid? CenterId { get; private set; }
        public string BatchName { get; private set; } = default!;
        public DateTime RegistrationStartDate { get; private set; }
        public DateTime RegistrationEndDate { get; private set; }
        public DateTime ExamStartDate { get; private set; }
        public int CurrentCandidates { get; private set; }
        public int MaxCandidates { get; private set; }
        public ExamBatchStatus Status { get; private set; }

        public virtual Center? Center { get; private set; }

        protected ExamBatch() { }

        public ExamBatch(
            ExamBatchScopeType scopeType,
            Guid? centerId,
            string batchName,
            DateTime registrationStartDate,
            DateTime registrationEndDate,
            DateTime examStartDate,
            int maxCandidates,
            Guid? createdBy = null)
        {
            SetScope(scopeType, centerId);
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
            ExamBatchScopeType? scopeType,
            Guid? centerId,
            string? batchName,
            DateTime? regStart,
            DateTime? regEnd,
            DateTime? examStart,
            int? maxCandidates = null,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (scopeType.HasValue)
            {
                changed |= SetScope(scopeType.Value, centerId);
            }

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

        private bool SetScope(ExamBatchScopeType scopeType, Guid? centerId)
        {
            if (!Enum.IsDefined(typeof(ExamBatchScopeType), scopeType))
                throw new ArgumentException("Invalid exam batch scope type");

            if (scopeType == ExamBatchScopeType.Center && (!centerId.HasValue || centerId == Guid.Empty))
                throw new ArgumentException("Center exam batch must have a center");

            if (scopeType == ExamBatchScopeType.National)
                centerId = null;

            if (ScopeType == scopeType && CenterId == centerId)
                return false;

            ScopeType = scopeType;
            CenterId = centerId;
            return true;
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
