using System;

namespace dtc.Domain.Entities.Permissions
{
    /// <summary>
    /// Liên kết nhân viên–cơ sở (many-to-many), không đặt collection trên <see cref="User"/> hay <see cref="Center"/>.
    /// </summary>
    public class UserCenter
    {
        public Guid UserId { get; private set; }
        public Guid CenterId { get; private set; }

        protected UserCenter() { }

        public UserCenter(Guid userId, Guid centerId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required", nameof(userId));
            if (centerId == Guid.Empty)
                throw new ArgumentException("CenterId is required", nameof(centerId));

            UserId = userId;
            CenterId = centerId;
        }
    }
}
