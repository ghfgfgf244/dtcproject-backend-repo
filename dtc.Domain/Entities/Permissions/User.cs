using System;
using dtc.Domain.Entities;
using dtc.Domain.ValueObjects;

namespace dtc.Domain.Entities.Permissions
{
    public class User : BaseEntity
    {
        public string ClerkId { get; private set; } = string.Empty; // Identifies user in Clerk
        public Email Email { get; private set; } = default!;
        public string FullName { get; private set; } = string.Empty;
        public PhoneNumber Phone { get; private set; } = default!;
        public string? AvatarUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        public UserRole RoleId { get; private set; }

        protected User() { }

        public User(
            string clerkId,
            Email email,
            string fullName,
            PhoneNumber phone,
            UserRole roleId = UserRole.Student,
            string? avatarUrl = null,
            Guid? createdBy = null)
        {
            Id = Guid.NewGuid();
            SetClerkId(clerkId);
            SetEmail(email);
            SetFullName(fullName);
            SetPhone(phone);
            SetAvatarUrl(avatarUrl);
            RoleId = roleId;

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

        public void SyncFromClerk(string clerkId, string? fullName, string? avatarUrl, Guid? updatedBy = null)
        {
            bool changed = false;
            changed |= SetClerkId(clerkId);

            if (!string.IsNullOrWhiteSpace(fullName))
                changed |= SetFullName(fullName);
            
            if (avatarUrl != null)
                changed |= SetAvatarUrl(avatarUrl);
            
            if (changed)
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

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void UpdateRole(UserRole role)
        {
            if (RoleId != role)
            {
                RoleId = role;
            }
        }

        // =========================
        // Internal setters
        // =========================

        private bool SetClerkId(string clerkId)
        {
            var normalized = clerkId?.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("ClerkId is required");

            if (ClerkId == normalized)
                return false;

            ClerkId = normalized;
            return true;
        }

        private void SetEmail(Email email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
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
