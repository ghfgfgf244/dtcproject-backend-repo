using System;

namespace dtc.Domain.Entities.Classes
{
    /// <summary>
    /// Bảng trung gian ClassSchedule–Address (many-to-many). Lưu trong MongoDB.
    /// </summary>
    public class LearningLocation
    {
        public Guid Id { get; private set; }
        public Guid ClassScheduleId { get; private set; }
        public int AddressId { get; private set; }

        protected LearningLocation() { }

        public LearningLocation(Guid classScheduleId, int addressId)
        {
            if (classScheduleId == Guid.Empty)
                throw new ArgumentException("ClassScheduleId is required", nameof(classScheduleId));
            if (addressId <= 0)
                throw new ArgumentException("AddressId is required", nameof(addressId));

            Id = Guid.NewGuid();
            ClassScheduleId = classScheduleId;
            AddressId = addressId;
        }
    }
}
