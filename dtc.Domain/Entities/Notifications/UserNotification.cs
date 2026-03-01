using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Notifications
{
    public class UserNotification
    {
        public Guid NotificationId { get; private set; }
        public Guid UserId { get; private set; }
        public bool IsRead { get; private set; }

        protected UserNotification() { }

        public UserNotification(Guid notificationId, Guid userId)
        {
            NotificationId = notificationId;
            UserId = userId;
            IsRead = false;
        }

        public void MarkAsRead() => IsRead = true;
    }
}
