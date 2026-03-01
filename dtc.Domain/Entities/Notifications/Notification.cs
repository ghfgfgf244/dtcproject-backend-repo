using dtc.Domain.Entities.Permissions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Notifications
{
    public class Notification : BaseEntity
    {
        //public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public NotificationType Type { get; private set; }
        public Guid? CenterId { get; private set; }
        //public UserRole? RoleTarget { get; private set; }

        private readonly List<NotificationRole> _targetRoles = new();
        public IReadOnlyCollection<NotificationRole> TargetRoles => _targetRoles.AsReadOnly();


        //public Guid CreatedBy { get; private set; }
        //public DateTime CreatedAt { get; private set; }

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
            //CreatedBy = createdBy;
            CenterId = centerId;
            if (target != null)
            {
                foreach (var role in target.Distinct())
                    _targetRoles.Add(new NotificationRole(Id, role));
            }
            //CreatedAt = DateTime.UtcNow;
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
                UpdateRoles(targetRoles);
                changed = true;
            }

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        private void UpdateRoles(IEnumerable<UserRole> roles)
        {
            var newRoles = roles.Distinct().ToHashSet();

            // Remove roles not in new list
            _targetRoles.RemoveAll(r => !newRoles.Contains(r.Role));

            // Add new roles
            foreach (var role in newRoles)
            {
                if (_targetRoles.All(x => x.Role != role))
                    _targetRoles.Add(new NotificationRole(Id, role));
            }
        }

        public bool AddRole(UserRole role)
        {
            if (_targetRoles.Any(x => x.Role == role))
                return false;

            _targetRoles.Add(new NotificationRole(Id, role));
            return true;
        }

        public void RemoveRole(UserRole role)
        {
            var item = _targetRoles.FirstOrDefault(x => x.Role == role);
            if (item != null)
                _targetRoles.Remove(item);
        }

        public bool IsTargetFor(UserRole role)
        {
            return !_targetRoles.Any() || _targetRoles.Any(x => x.Role == role);
        }
    }
}
