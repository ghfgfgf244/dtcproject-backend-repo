using dtc.Domain.ValueObjects;

namespace dtc.Domain.Entities.Location
{
    public class Center : BaseEntity
    {
        public string CenterName { get; private set; } = default!;
        public string Address { get; private set; } = default!;
        public PhoneNumber Phone { get; private set; } = default!;
        public Email Email { get; private set; } = default!;
        public bool IsActive { get; private set; }
        public int NumberOfClasses { get; private set; }
        public int MaxStudentPerClass { get; private set; }


        protected Center() { }

        public Center(
            string name,
            string address,
            PhoneNumber phone,
            Email email,
            int numberOfClasses,
            int maxStudentPerClass,
            Guid? createdBy = null)
        {
            Id = Guid.NewGuid();

            SetName(name);
            SetAddress(address);

            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));

            SetClassLimits(numberOfClasses, maxStudentPerClass);

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

        public void UpdateClassLimits(int numberOfClasses, int maxStudentPerClass, Guid? updatedBy = null)
        {
            if (NumberOfClasses == numberOfClasses && MaxStudentPerClass == maxStudentPerClass)
                return;

            SetClassLimits(numberOfClasses, maxStudentPerClass);
            SetUpdated(updatedBy);
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

        private void SetClassLimits(int numberOfClasses, int maxStudentPerClass)
        {
            if (numberOfClasses < 0)
                throw new ArgumentException("NumberOfClasses cannot be negative");
            if (maxStudentPerClass <= 0)
                throw new ArgumentException("MaxStudentPerClass must be greater than 0");

            NumberOfClasses = numberOfClasses;
            MaxStudentPerClass = maxStudentPerClass;
        }
    }
}
