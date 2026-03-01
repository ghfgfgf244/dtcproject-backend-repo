using dtc.Domain.Entities.Notifications;
using dtc.Domain.Interfaces.Notifications;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Notifications
{
    public class UserNotificationRepository : MongoGenericRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(MongoDBContext context) : base(context, "UserNotifications")
        {
        }
    }
}
