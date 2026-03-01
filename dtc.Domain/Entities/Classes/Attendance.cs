namespace dtc.Domain.Entities.Classes;

public class Attendance : BaseEntity
{
    public Guid ClassScheduleId { get; private set; }
    public Guid StudentId { get; private set; }

    public bool IsPresent { get; private set; }
    public DateTime CheckedAt { get; private set; }

    protected Attendance() { }

    // =========================
    // CREATE
    // =========================
    public Attendance(
        Guid classScheduleId,
        Guid studentId,
        bool isPresent,
        Guid? createdBy = null)
    {
        if (classScheduleId == Guid.Empty)
            throw new ArgumentException("ClassScheduleId is required");

        if (studentId == Guid.Empty)
            throw new ArgumentException("StudentId is required");

        ClassScheduleId = classScheduleId;
        StudentId = studentId;

        IsPresent = isPresent;
        CheckedAt = DateTime.UtcNow;

        SetCreated(createdBy);
    }

    // =========================
    // UPDATE
    // =========================
    public bool UpdateAttendance(
        bool isPresent,
        Guid? updatedBy = null)
    {
        if (IsPresent == isPresent)
            return false;

        IsPresent = isPresent;
        CheckedAt = DateTime.UtcNow;

        SetUpdated(updatedBy);
        return true;
    }

    // =========================
    // HELPER
    // =========================
    public void MarkPresent(Guid? updatedBy = null)
        => UpdateAttendance(true, updatedBy);

    public void MarkAbsent(Guid? updatedBy = null)
        => UpdateAttendance(false, updatedBy);
}
