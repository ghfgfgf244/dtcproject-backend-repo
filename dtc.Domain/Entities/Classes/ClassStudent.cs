using System;

namespace dtc.Domain.Entities.Classes
{
    /// <summary>
    /// Đăng ký học viên vào lớp (many-to-many), không đặt collection học viên trên <see cref="Class"/>.
    /// </summary>
    public class ClassStudent
    {
        public Guid ClassId { get; private set; }
        public Guid StudentId { get; private set; }

        protected ClassStudent() { }

        public ClassStudent(Guid classId, Guid studentId)
        {
            if (classId == Guid.Empty)
                throw new ArgumentException("ClassId is required", nameof(classId));
            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId is required", nameof(studentId));

            ClassId = classId;
            StudentId = studentId;
        }
    }
}
