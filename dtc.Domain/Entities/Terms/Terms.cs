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

        protected Term() { }

        public Term(
            Guid courseId, 
            string termName, 
            DateTime startDate, 
            DateTime endDate, 
            Guid? createdBy = null)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId is required");

            CourseId = courseId;
            SetName(termName);
            SetSchedule(startDate, endDate);
            
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

            if (!changed)
                return false;

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
    }
}
