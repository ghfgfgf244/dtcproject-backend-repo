using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Training
{
    public class Course : BaseEntity
    {
        public Guid CenterId { get; private set; }
        public string CourseName { get; private set; }
        public ExamLevel LicenseType { get; private set; }
        public int DurationInWeeks { get; private set; }
        public int MaxStudents { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }

        protected Course() { }

        public Course(Guid centerId, string name, ExamLevel licenseType, int duration, int maxStudent, string? thumbnailUrl, string description, decimal price, Guid? createBy=null)
        {
            Id = Guid.NewGuid();
            SetCenter(centerId);
            SetName(name);
            SetLicenseType(licenseType);
            SetDuration(duration);
            SetMaxStudents(maxStudent);
            SetPrice(price);
            SetDescription(description);
            SetThumbnail(thumbnailUrl);

            IsActive = true;
            SetCreated(createBy);
        }
        public bool UpdateInfo(
            string? name,
            string? description,
            string? thumbnailUrl,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(name))
                changed |= SetName(name);

            if (!string.IsNullOrWhiteSpace(description))
                changed |= SetDescription(description);

            if (thumbnailUrl is not null)
                changed |= SetThumbnail(thumbnailUrl);

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public bool ChangePrice(decimal newPrice, Guid? updatedBy = null)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than 0");

            if (Price == newPrice)
                return false;

            Price = newPrice;
            SetUpdated(updatedBy);
            return true;
        }

        public bool ChangeCapacity(int maxStudents, Guid? updatedBy = null)
        {
            if (maxStudents <= 0)
                throw new ArgumentException("MaxStudents must be greater than 0");

            if (MaxStudents == maxStudents)
                return false;

            MaxStudents = maxStudents;
            SetUpdated(updatedBy);
            return true;
        }

        public bool ChangeDuration(int durationInWeeks, Guid? updatedBy = null)
        {
            if (durationInWeeks <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            if (DurationInWeeks == durationInWeeks)
                return false;

            DurationInWeeks = durationInWeeks;
            SetUpdated(updatedBy);
            return true;
        }

        public void Activate(Guid? updatedBy = null)
        {
            if (IsActive) return;

            IsActive = true;
            SetUpdated(updatedBy);
        }

        public void Deactivate(bool hasActiveClasses, Guid? updatedBy = null)
        {
            if (hasActiveClasses)
                throw new InvalidOperationException("Cannot deactivate course with active classes");

            if (!IsActive) return;

            IsActive = false;
            SetUpdated(updatedBy);
        }

        // =========================
        // Internal setters
        // =========================

        private void SetCenter(Guid centerId)
        {
            if (centerId == Guid.Empty)
                throw new ArgumentException("CenterId is required");

            CenterId = centerId;
        }

        private bool SetName(string name)
        {
            var normalized = name.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("CourseName is required");

            if (CourseName == normalized)
                return false;

            CourseName = normalized;
            return true;
        }

        private bool SetLicenseType(ExamLevel licenseType)
        {
            if (!Enum.IsDefined(typeof(ExamLevel), licenseType))
                throw new ArgumentException("Invalid license type");

            if (LicenseType == licenseType)
                return false;

            LicenseType = licenseType;
            return true;
        }

        private bool SetDuration(int duration)
        {
            if (duration <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            if (DurationInWeeks == duration)
                return false;

            DurationInWeeks = duration;
            return true;
        }

        private bool SetMaxStudents(int maxStudents)
        {
            if (maxStudents <= 0)
                throw new ArgumentException("MaxStudents must be greater than 0");

            if (MaxStudents == maxStudents)
                return false;

            MaxStudents = maxStudents;
            return true;
        }

        private bool SetPrice(decimal price)
        {
            if (price <= 0)
                throw new ArgumentException("Price must be greater than 0");

            if (Price == price)
                return false;

            Price = price;
            return true;
        }

        private bool SetDescription(string description)
        {
            var normalized = description.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("Description is required");

            if (Description == normalized)
                return false;

            Description = normalized;
            return true;
        }

        private bool SetThumbnail(string? thumbnailUrl)
        {
            var normalized = thumbnailUrl?.Trim();

            if (ThumbnailUrl == normalized)
                return false;

            ThumbnailUrl = normalized;
            return true;
        }
    }

}
