using System;

namespace dtc.Domain.Entities.Classes
{
    public class Class : BaseEntity
    {
        public Guid TermId { get; private set; }

        /// <summary>Giáo viên chủ nhiệm (một lớp — một GV; một GV có thể chủ nhiệm nhiều lớp).</summary>
        public Guid InstructorId { get; private set; }

        public string ClassName { get; private set; } = default!;
        public int CurrentStudents { get; private set; }
        public int MaxStudents { get; set; }
        public ClassType ClassType { get; private set; }
        public ClassStatus Status { get; private set; }

        protected Class() { }

        // =========================
        // CREATE
        // =========================
        public Class(
            Guid termId,
            Guid instructorId,
            string className,
            ClassType classType,
            int maxStudents,
            Guid? createdBy = null)
        {
            if (termId == Guid.Empty)
                throw new ArgumentException("TermId is required");
            if (instructorId == Guid.Empty)
                throw new ArgumentException("InstructorId is required");

            SetName(className);
            SetClassType(classType);
            SetMaxStudents(maxStudents);

            TermId = termId;
            InstructorId = instructorId;
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

        public void UpdateStudentCount(int count, Guid? updatedBy = null)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            CurrentStudents = count;
            SetUpdated(updatedBy);
        }

        /// <summary>Đồng bộ sĩ số với bảng liên kết học viên–lớp (được cập nhật từ Application/Infrastructure).</summary>
        public void SyncEnrollmentCount(int count, Guid? updatedBy = null)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count > MaxStudents)
                throw new InvalidOperationException("Enrollment count cannot exceed MaxStudents");

            CurrentStudents = count;
            SetUpdated(updatedBy);
        }

        public bool UpdateInfo(
            string? className,
            ClassType? classType,
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

            if (classType.HasValue && classType.Value != ClassType)
            {
                SetClassType(classType.Value);
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

        public void ChangeInstructor(Guid newInstructorId, Guid? updatedBy = null)
        {
            if (newInstructorId == Guid.Empty)
                throw new ArgumentException("InstructorId is required");
            if (InstructorId == newInstructorId) return;

            InstructorId = newInstructorId;
            SetUpdated(updatedBy);
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

        private void SetClassType(ClassType classType)
        {
            if (!Enum.IsDefined(typeof(ClassType), classType))
                throw new ArgumentException("ClassType is invalid");

            ClassType = classType;
        }
    }
}
