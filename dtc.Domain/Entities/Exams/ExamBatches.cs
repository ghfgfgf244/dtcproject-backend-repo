using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Exams
{
    public class ExamBatch : BaseEntity
    {
        public Guid CourseId { get; private set; }
        public string BatchName { get; private set; } = default!;
        public DateTime RegistrationStartDate { get; private set; }
        public DateTime RegistrationEndDate { get; private set; }
        public DateTime ExamStartDate { get; private set; }
        public ExamBatchStatus Status { get; private set; }

        protected ExamBatch() { }

        public ExamBatch(
            Guid courseId,
            string batchName,
            DateTime registrationStartDate,
            DateTime registrationEndDate,
            DateTime examStartDate,
            Guid? createdBy = null)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId is required");

            CourseId = courseId;
            SetBatchName(batchName);
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
    }
}
