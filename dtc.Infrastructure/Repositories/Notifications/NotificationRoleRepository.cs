using dtc.Domain.Entities.Notifications;
using dtc.Domain.Interfaces.Notifications;
using dtc.Infrastructure.Persistence.MongoDB;

namespace dtc.Infrastructure.Repositories.Notifications
{
    public class NotificationRoleRepository : MongoGenericRepository<NotificationRole>, INotificationRoleRepository
    {
        public NotificationRoleRepository(MongoDBContext context) : base(context, "NotificationRoles")
        {
        }
    }
}
