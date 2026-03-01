using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Terms
{
    public class CourseRegistration : BaseEntity
    {
        public Guid CourseId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public CourseRegistrationStatus Status { get; private set; }
        public decimal TotalFee { get; private set; }
        public string? Notes { get; private set; }

        protected CourseRegistration() { }

        public CourseRegistration(Guid courseId, Guid userId, decimal totalFee, string? notes = null, Guid? createdBy = null)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId is required");

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (totalFee < 0)
                throw new ArgumentException("Total fee cannot be negative");

            Id = Guid.NewGuid();
            CourseId = courseId;
            UserId = userId;
            TotalFee = totalFee;
            RegistrationDate = DateTime.UtcNow;
            Status = CourseRegistrationStatus.Pending;
            Notes = notes?.Trim();

            SetCreated(createdBy);
        }

        // =========================
        // BEHAVIORS
        // =========================

        public void Approve(Guid? updatedBy = null)
        {
            if (Status != CourseRegistrationStatus.Pending)
                throw new InvalidOperationException("Can only approve pending registrations");

            Status = CourseRegistrationStatus.Approved;
            SetUpdated(updatedBy);
        }

        public void Reject(string reason, Guid? updatedBy = null)
        {
            if (Status != CourseRegistrationStatus.Pending)
                throw new InvalidOperationException("Can only reject pending registrations");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Must provide a reason for rejection");

            Status = CourseRegistrationStatus.Rejected;
            UpdateNotes(reason, updatedBy);
            SetUpdated(updatedBy);
        }

        public void Cancel(string reason, Guid? updatedBy = null)
        {
            if (Status == CourseRegistrationStatus.Cancelled || Status == CourseRegistrationStatus.Rejected)
                throw new InvalidOperationException("Registration is already cancelled or rejected");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Must provide a reason for cancellation");

            Status = CourseRegistrationStatus.Cancelled;
            UpdateNotes(reason, updatedBy);
            SetUpdated(updatedBy);
        }

        public bool UpdateNotes(string notes, Guid? updatedBy = null)
        {
            var normalized = notes?.Trim();
            if (Notes == normalized)
                return false;

            Notes = normalized;
            SetUpdated(updatedBy);
            return true;
        }
    }
}
