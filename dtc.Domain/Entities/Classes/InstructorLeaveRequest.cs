using System;

namespace dtc.Domain.Entities.Classes
{
    public enum LeaveRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

    public class InstructorLeaveRequest : BaseEntity
    {
        public Guid InstructorId { get; private set; }
        public DateTime LeaveDate { get; private set; }
        public string Reason { get; private set; } = default!;
        public LeaveRequestStatus Status { get; private set; }

        protected InstructorLeaveRequest() { }

        public InstructorLeaveRequest(Guid instructorId, DateTime leaveDate, string reason, Guid? createdBy = null)
        {
            if (instructorId == Guid.Empty)
                throw new ArgumentException("InstructorId is required");
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason is required");

            Id = Guid.NewGuid();
            InstructorId = instructorId;
            LeaveDate = leaveDate;
            Reason = reason.Trim();
            Status = LeaveRequestStatus.Pending;

            SetCreated(createdBy);
        }

        public void ChangeStatus(LeaveRequestStatus newStatus, Guid? updatedBy = null)
        {
            if (Status == newStatus) return;

            Status = newStatus;
            SetUpdated(updatedBy);
        }
    }
}
