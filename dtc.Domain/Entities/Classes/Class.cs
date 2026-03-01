using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Classes
{
    public class Class : BaseEntity
    {
        public Guid TermId { get; private set; }
        public string ClassName { get; private set; } = default!;

        public int CurrentStudents { get; private set; }
        public int MaxStudents { get; private set; }
        public ClassStatus Status { get; private set; }

        protected Class() { }

        // =========================
        // CREATE
        // =========================
        public Class(
            Guid termId,
            string className,
            int maxStudents,
            Guid? createdBy = null)
        {
            if (termId == Guid.Empty)
                throw new ArgumentException("TermId is required");

            SetName(className);
            SetMaxStudents(maxStudents);

            TermId = termId;
            CurrentStudents = 0;
            Status = ClassStatus.Pending;

            Id = Guid.NewGuid();
            SetCreated(createdBy);
        }

        // =========================
        // BEHAVIOR
        // =========================
        public bool EnrollStudent(Guid? updatedBy = null)
        {
            if (Status != ClassStatus.Pending && Status != ClassStatus.InProgress)
                throw new InvalidOperationException("Cannot enroll in this class status");

            if (IsFull())
                throw new InvalidOperationException("Class is full");

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

        public bool UpdateInfo(
            string? className,
            int? maxStudents,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(className) &&
                ClassName != className.Trim())
            {
                ClassName = className.Trim();
                changed = true;
            }

            if (maxStudents.HasValue && maxStudents.Value != MaxStudents)
            {
                if (maxStudents.Value < CurrentStudents)
                    throw new InvalidOperationException("MaxStudents cannot be less than current students");

                SetMaxStudents(maxStudents.Value);
                changed = true;
            }

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public void ChangeStatus(ClassStatus newStatus, Guid? updatedBy = null)
        {
            if (Status == newStatus) return;

            Status = newStatus;
            SetUpdated(updatedBy);
        }

        public bool IsFull() => CurrentStudents >= MaxStudents;

        // =========================
        // PRIVATE RULES
        // =========================
        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Class name is required");

            ClassName = name.Trim();
        }

        private void SetMaxStudents(int max)
        {
            if (max <= 0)
                throw new ArgumentException("MaxStudents must be greater than 0");

            MaxStudents = max;
        }
    }
}
