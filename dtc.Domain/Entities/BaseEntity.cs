namespace dtc.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public bool IsDeleted { get; private set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected void SetCreated(Guid? userId)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userId;
    }

    protected void SetUpdated(Guid? userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }

    public void SoftDelete(Guid? userId = null)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        SetUpdated(userId);
    }

    public void Restore(Guid? userId = null)
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        SetUpdated(userId);
    }
}
