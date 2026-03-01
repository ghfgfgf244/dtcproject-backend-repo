using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Collaborators
{
    public class CollaboratorCommission
    {
        public Guid Id { get; private set; }
        public Guid CollaboratorId { get; private set; }
        public decimal Amount { get; private set; }
        public CommissionStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PaidAt { get; private set; }

        protected CollaboratorCommission() { }

        public CollaboratorCommission(Guid collaboratorId, decimal amount)
        {
            if (collaboratorId == Guid.Empty)
                throw new ArgumentException("CollaboratorId is required");

            if (amount <= 0)
                throw new ArgumentException("Commission amount must be greater than 0");

            Id = Guid.NewGuid();
            CollaboratorId = collaboratorId;
            Amount = amount;

            Status = CommissionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsPaid()
        {
            if (Status == CommissionStatus.Paid)
                throw new InvalidOperationException("Commission already paid");

            if (Status == CommissionStatus.Cancelled)
                throw new InvalidOperationException("Cannot pay a cancelled commission");

            Status = CommissionStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == CommissionStatus.Paid)
                throw new InvalidOperationException("Paid commission cannot be cancelled");

            Status = CommissionStatus.Cancelled;
        }
    }
}
