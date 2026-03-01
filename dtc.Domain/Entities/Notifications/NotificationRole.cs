
namespace dtc.Domain.Entities.Notifications;

public class NotificationRole
{
    public Guid NotificationId { get; private set; }
    public UserRole Role { get; private set; }

    protected NotificationRole() { }

    public NotificationRole(Guid notificationId, UserRole role)
    {
        if (notificationId == Guid.Empty)
            throw new ArgumentException("NotificationId is required", nameof(notificationId));

        NotificationId = notificationId;
        Role = role;
    }
}
