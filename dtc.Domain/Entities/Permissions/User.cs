using dtc.Domain.ValueObjects;

namespace dtc.Domain.Entities.Permissions
{
    public class User : BaseEntity
    {
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public PhoneNumber Phone { get; private set; }
        public string? AvatarUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        private readonly List<Role> _roles = new();
        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

        protected User() { }

        public User(
            Email email,
            string passwordHash,
            string fullName,
            PhoneNumber phone,
            string? avatarUrl = null,
            Guid? createdBy = null)
        {
            Id = Guid.NewGuid();

            SetEmail(email);
            SetPasswordHash(passwordHash);
            SetFullName(fullName);
            SetPhone(phone);
            SetAvatarUrl(avatarUrl);

            IsActive = true;
            SetCreated(createdBy);
        }

        // =========================
        // Behaviors
        // =========================

        public bool UpdateProfile(
            string? fullName,
            PhoneNumber? phone,
            string? avatarUrl,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(fullName))
                changed |= SetFullName(fullName);

            if (phone is not null)
                changed |= SetPhone(phone);

            if (avatarUrl is not null)
                changed |= SetAvatarUrl(avatarUrl);

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public bool ChangePassword(string newPasswordHash, Guid? updatedBy = null)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("PasswordHash is required");

            if (PasswordHash == newPasswordHash)
                return false;

            PasswordHash = newPasswordHash;
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

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void AddRole(Role role)
        {
            if (role != null && !_roles.Contains(role))
            {
                _roles.Add(role);
            }
        }

        public void RemoveRole(Role role)
        {
            if (role != null)
            {
                _roles.Remove(role);
            }
        }

        // =========================
        // Internal setters
        // =========================

        private void SetEmail(Email email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        private void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required");

            PasswordHash = passwordHash;
        }

        private bool SetFullName(string fullName)
        {
            var normalized = fullName.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("FullName is required");

            if (FullName == normalized)
                return false;

            FullName = normalized;
            return true;
        }

        private bool SetPhone(PhoneNumber phone)
        {
            if (Phone == phone)
                return false;

            Phone = phone;
            return true;
        }

        private bool SetAvatarUrl(string? avatarUrl)
        {
            var normalized = avatarUrl?.Trim();

            if (AvatarUrl == normalized)
                return false;

            AvatarUrl = normalized;
            return true;
        }
    }
}
