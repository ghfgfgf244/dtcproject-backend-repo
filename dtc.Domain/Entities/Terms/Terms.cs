using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Terms
{
    public class Term : BaseEntity
    {
        public Guid CourseId { get; private set; }
        public string TermName { get; private set; } = default!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int CurrentStudents { get; private set; }
        public int MaxStudents { get; private set; }
        public bool IsActive { get; private set; }

        protected Term() { }

        public Term(
            Guid courseId, 
            string termName, 
            DateTime startDate, 
            DateTime endDate, 
            int maxStudents,
            Guid? createdBy = null)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId is required");

            CourseId = courseId;
            SetName(termName);
            SetSchedule(startDate, endDate);
            SetMaxStudents(maxStudents);
            CurrentStudents = 0;
            IsActive = true;
            
            Id = Guid.NewGuid();
            SetCreated(createdBy);
        }

        // =========================
        // BEHAVIORS
        // =========================

        public bool UpdateInfo(
            string? termName,
            DateTime? startDate,
            DateTime? endDate,
            int? maxStudents = null,
            bool? isActive = null,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(termName) && TermName != termName.Trim())
            {
                TermName = termName.Trim();
                changed = true;
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                var newStart = startDate ?? StartDate;
                var newEnd = endDate ?? EndDate;
                
                SetSchedule(newStart, newEnd);
                changed = true;
            }

            if (maxStudents.HasValue && MaxStudents != maxStudents.Value)
            {
                SetMaxStudents(maxStudents.Value);
                changed = true;
            }

            if (isActive.HasValue && IsActive != isActive.Value)
            {
                IsActive = isActive.Value;
                changed = true;
            }

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public void Activate(Guid? updatedBy = null)
        {
            if (IsActive) return;
            IsActive = true;
            SetUpdated(updatedBy);
        }

        public void Deactivate(Guid? updatedBy = null)
        {
            if (!IsActive) return;
            IsActive = false;
            SetUpdated(updatedBy);
        }

        public bool EnrollStudent(Guid? updatedBy = null)
        {
            if (CurrentStudents >= MaxStudents)
                throw new InvalidOperationException("Term is full");

            CurrentStudents++;
            SetUpdated(updatedBy);
            return true;
        }

        public bool RemoveStudent(Guid? updatedBy = null)
        {
            if (CurrentStudents <= 0)
                return false;

            CurrentStudents--;
            SetUpdated(updatedBy);
            return true;
        }

        // =========================
        // PRIVATE RULES
        // =========================

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("TermName is required");

            TermName = name.Trim();
        }

        private void SetSchedule(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("StartDate must be before EndDate");

            StartDate = start;
            EndDate = end;
        }

        private void SetMaxStudents(int max)
        {
            if (max <= 0)
                throw new ArgumentException("MaxStudents must be greater than 0");

            MaxStudents = max;
        }
    }
}
