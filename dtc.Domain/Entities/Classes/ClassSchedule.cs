namespace dtc.Domain.Entities.Classes;

public class ClassSchedule : BaseEntity
{
    public Guid ClassId { get; private set; }
    public Guid InstructorId { get; private set; }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public string Location { get; private set; } = default!;

    protected ClassSchedule() { }

    // =========================
    // CREATE
    // =========================
    public ClassSchedule(
        Guid classId,
        Guid instructorId,
        DateTime startTime,
        DateTime endTime,
        string location,
        Guid? createdBy)
    {
        if (classId == Guid.Empty)
            throw new ArgumentException("ClassId is required");

        if (instructorId == Guid.Empty)
            throw new ArgumentException("InstructorId is required");

        SetSchedule(startTime, endTime);
        SetLocation(location);

        ClassId = classId;
        InstructorId = instructorId;

        SetCreated(createdBy);
    }

    // =========================
    // BEHAVIOR
    // =========================
    public bool Reschedule(
        DateTime newStart,
        DateTime newEnd,
        string? newLocation,
        Guid? updatedBy)
    {
        bool changed = false;

        if (newStart != StartTime || newEnd != EndTime)
        {
            SetSchedule(newStart, newEnd);
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(newLocation) &&
            !string.Equals(Location, newLocation.Trim(), StringComparison.Ordinal))
        {
            Location = newLocation.Trim();
            changed = true;
        }

        if (!changed)
            return false;

        SetUpdated(updatedBy);
        return true;
    }

    public void ChangeInstructor(Guid newInstructorId, Guid? updatedBy)
    {
        if (newInstructorId == Guid.Empty)
            throw new ArgumentException("InstructorId is required");

        if (newInstructorId == InstructorId)
            return;

        InstructorId = newInstructorId;
        SetUpdated(updatedBy);
    }

    public bool IsOverlapping(DateTime start, DateTime end)
    {
        return StartTime < end && start < EndTime;
    }

    // =========================
    // PRIVATE RULES
    // =========================
    private void SetSchedule(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new ArgumentException("StartTime must be before EndTime");

        StartTime = start;
        EndTime = end;
    }

    private void SetLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location is required");

        Location = location.Trim();
    }
}
