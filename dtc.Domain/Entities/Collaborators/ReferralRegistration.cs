namespace dtc.Domain.Entities.Collaborators
{
    public class ReferralRegistration
    {
        public Guid Id { get; private set; }
        public Guid ReferralCodeId { get; private set; }
        public Guid StudentId { get; private set; }
        public DateTime RegisteredAt { get; private set; }

        protected ReferralRegistration() { }

        public ReferralRegistration(Guid referralCodeId, Guid studentId)
        {
            if (referralCodeId == Guid.Empty)
                throw new ArgumentException("ReferralCodeId is required");

            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId is required");

            Id = Guid.NewGuid();
            ReferralCodeId = referralCodeId;
            StudentId = studentId;
            RegisteredAt = DateTime.UtcNow;
        }

    }
}
