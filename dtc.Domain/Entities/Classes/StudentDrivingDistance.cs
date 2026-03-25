using System;

namespace dtc.Domain.Entities.Classes
{
    public class StudentDrivingDistance : BaseEntity
    {
        public Guid StudentId { get; private set; }
        public double MorningDistanceKm { get; private set; }
        public double EveningDistanceKm { get; private set; }
        
        public double MaxMorningDistanceKm { get; private set; }
        public double MaxEveningDistanceKm { get; private set; }

        protected StudentDrivingDistance() { }

        public StudentDrivingDistance(Guid studentId, double maxMorningDistanceKm, double maxEveningDistanceKm, Guid? createdBy = null)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId is required");
            if (maxMorningDistanceKm < 0 || maxEveningDistanceKm < 0)
                throw new ArgumentException("Max distances cannot be negative");

            Id = Guid.NewGuid();
            StudentId = studentId;
            MaxMorningDistanceKm = maxMorningDistanceKm;
            MaxEveningDistanceKm = maxEveningDistanceKm;
            MorningDistanceKm = 0;
            EveningDistanceKm = 0;

            SetCreated(createdBy);
        }

        public void AddMorningDistance(double distance, Guid? updatedBy = null)
        {
            if (distance < 0) throw new ArgumentException("Cannot add negative distance");
            MorningDistanceKm += distance;
            SetUpdated(updatedBy);
        }

        public void AddEveningDistance(double distance, Guid? updatedBy = null)
        {
            if (distance < 0) throw new ArgumentException("Cannot add negative distance");
            EveningDistanceKm += distance;
            SetUpdated(updatedBy);
        }
    }
}
