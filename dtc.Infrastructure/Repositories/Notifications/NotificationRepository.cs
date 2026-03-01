using dtc.Domain.Entities.Notifications;
using dtc.Domain.Interfaces.Notifications;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Notifications
{
    public class NotificationRepository : MongoGenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(MongoDBContext context) : base(context, "Notifications")
        {
        }
    }
}
