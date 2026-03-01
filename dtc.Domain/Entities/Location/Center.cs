using dtc.Domain.ValueObjects;

namespace dtc.Domain.Entities.Permissions
{
    public class Center : BaseEntity
    {
        public string CenterName { get; private set; } = default!;
        public string Address { get; private set; } = default!;
        public PhoneNumber Phone { get; private set; } = default!;
        public Email Email { get; private set; } = default!;
        public bool IsActive { get; private set; }

        protected Center() { }

        public Center(
            string name,
            string address,
            PhoneNumber phone,
            Email email,
            Guid? createdBy = null)
        {
            Id = Guid.NewGuid();

            SetName(name);
            SetAddress(address);

            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));

            IsActive = true;
            SetCreated(createdBy);
        }

        // =========================
        // Behaviors
        // =========================

        public bool UpdateInfo(
            string? name,
            string? address,
            PhoneNumber? phone,
            Email? email,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(name))
                changed |= SetName(name);

            if (!string.IsNullOrWhiteSpace(address))
                changed |= SetAddress(address);

            if (phone != null && !Phone.Equals(phone))
            {
                Phone = phone;
                changed = true;
            }

            if (email != null && !Email.Equals(email))
            {
                Email = email;
                changed = true;
            }

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public void Activate(Guid? updatedBy = null)
        {
            if (IsActive) return;
            IsActive = true;
            SetUpdated(updatedBy);
        }

        public void Deactivate(Guid? updatedBy = null)
        {
            if (!IsActive) return;
            IsActive = false;
            SetUpdated(updatedBy);
        }

        // =========================
        // Internal setters
        // =========================

        private bool SetName(string name)
        {
            var normalized = name.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("CenterName is required");

            if (CenterName == normalized)
                return false;

            CenterName = normalized;
            return true;
        }

        private bool SetAddress(string address)
        {
            var normalized = address.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("Address is required");

            if (Address == normalized)
                return false;

            Address = normalized;
            return true;
        }
    }
}
