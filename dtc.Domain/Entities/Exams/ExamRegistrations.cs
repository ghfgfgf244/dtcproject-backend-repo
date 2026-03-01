using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Exams
{
    public class ExamRegistration : BaseEntity
    {
        public Guid ExamBatchId { get; private set; }
        public Guid StudentId { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public bool IsPaid { get; private set; }
        public ExamRegistrationStatus Status { get; private set; }

        protected ExamRegistration() { }

        public ExamRegistration(
            Guid examBatchId,
            Guid studentId,
            bool isPaid = false,
            Guid? createdBy = null)
        {
            if (examBatchId == Guid.Empty)
                throw new ArgumentException("ExamBatchId is required");

            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId is required");

            Id = Guid.NewGuid();
            ExamBatchId = examBatchId;
            StudentId = studentId;
            RegistrationDate = DateTime.UtcNow;
            IsPaid = isPaid;
            Status = ExamRegistrationStatus.Pending;

            SetCreated(createdBy);
        }

        // =========================
        // BEHAVIORS
        // =========================

        public void MarkAsPaid(Guid? updatedBy = null)
        {
            if (IsPaid) return;

            IsPaid = true;
            SetUpdated(updatedBy);
        }

        public void Approve(Guid? updatedBy = null)
        {
            if (Status != ExamRegistrationStatus.Pending)
                throw new InvalidOperationException("Can only approve pending registrations");

            Status = ExamRegistrationStatus.Approved;
            SetUpdated(updatedBy);
        }

        public void Reject(Guid? updatedBy = null)
        {
            if (Status != ExamRegistrationStatus.Pending)
                throw new InvalidOperationException("Can only reject pending registrations");

            Status = ExamRegistrationStatus.Rejected;
            SetUpdated(updatedBy);
        }

        public void Cancel(Guid? updatedBy = null)
        {
            if (Status == ExamRegistrationStatus.Cancelled || Status == ExamRegistrationStatus.Rejected)
                throw new InvalidOperationException("Registration is already cancelled or rejected");

            Status = ExamRegistrationStatus.Cancelled;
            SetUpdated(updatedBy);
        }
    }
}
