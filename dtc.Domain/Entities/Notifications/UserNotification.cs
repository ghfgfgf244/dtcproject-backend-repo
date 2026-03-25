using System;

namespace dtc.Domain.Entities.Notifications
{
    /// <summary>Đánh dấu đã đọc thông báo. Lưu trong MongoDB.</summary>
    public class UserNotification
    {
        public Guid Id { get; private set; }
        public Guid NotificationId { get; private set; }
        public Guid UserId { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }

        protected UserNotification() { }

        public UserNotification(Guid notificationId, Guid userId)
        {
            if (notificationId == Guid.Empty)
                throw new ArgumentException("NotificationId is required");
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            Id = Guid.NewGuid();
            NotificationId = notificationId;
            UserId = userId;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            if (IsRead) return;
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
