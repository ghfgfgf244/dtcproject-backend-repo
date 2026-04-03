using System;
using System.Collections.Generic;
using System.Linq;
using dtc.Domain.Entities;

namespace dtc.Domain.Entities.Notifications
{
    /// <summary>
    /// Notification lưu trong MongoDB. TargetRoles nhúng trực tiếp trong document (không dùng bảng trung gian).
    /// </summary>
    public class Notification : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public NotificationType Type { get; private set; }
        public Guid? CenterId { get; private set; }
        public List<UserRole> TargetRoles { get; private set; } = new();

        protected Notification() { }

        public Notification(
            string title,
            string content,
            NotificationType type,
            Guid createdBy,
            Guid? centerId = null,
            IEnumerable<UserRole>? target = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content is required");

            Id = Guid.NewGuid();
            Title = title;
            Content = content;
            Type = type;
            CenterId = centerId;

            if (target != null)
            {
                foreach (var role in target.Distinct())
                    TargetRoles.Add(role);
            }

            SetCreated(createdBy);
        }

        public bool Update(
            string? title,
            string? content,
            NotificationType? type,
            Guid? centerId,
            IEnumerable<UserRole>? targetRoles,
            Guid updatedBy)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(title) &&
                !string.Equals(Title, title, StringComparison.Ordinal))
            {
                Title = title.Trim();
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(content) &&
                !string.Equals(Content, content, StringComparison.Ordinal))
            {
                Content = content.Trim();
                changed = true;
            }

            if (type.HasValue && Type != type.Value)
            {
                Type = type.Value;
                changed = true;
            }

            if (CenterId != centerId)
            {
                CenterId = centerId;
                changed = true;
            }

            if (targetRoles != null)
            {
                TargetRoles.Clear();
                foreach (var role in targetRoles.Distinct())
                    TargetRoles.Add(role);
                changed = true;
            }

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public bool AddRole(UserRole role)
        {
            if (TargetRoles.Contains(role))
                return false;

            TargetRoles.Add(role);
            return true;
        }

        public void RemoveRole(UserRole role)
        {
            TargetRoles.Remove(role);
        }

        public bool IsTargetFor(UserRole role)
        {
            return TargetRoles.Count == 0 || TargetRoles.Contains(role);
        }
    }
}
