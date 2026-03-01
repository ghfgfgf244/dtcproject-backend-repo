namespace dtc.Domain.Entities.Collaborators;

public class ReferralCode : BaseEntity
{
    public string Code { get; private set; }
    public Guid CollaboratorId { get; private set; }

    public int UsedCount { get; private set; }
    public bool IsActive { get; private set; }

    protected ReferralCode() { }

    // =========================
    // CREATE
    // =========================
    public ReferralCode(
        string code,
        Guid collaboratorId,
        Guid? createdBy = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Referral code is required");

        if (collaboratorId == Guid.Empty)
            throw new ArgumentException("CollaboratorId is required");

        Id = Guid.NewGuid();
        Code = code.Trim().ToUpper(); // tránh trùng ABC vs abc
        CollaboratorId = collaboratorId;

        UsedCount = 0;
        IsActive = true;

        SetCreated(createdBy);
    }

    // =========================
    // BEHAVIOR
    // =========================
    public void Deactivate(Guid? updatedBy = null)
    {
        if (!IsActive) return;

        IsActive = false;
        SetUpdated(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        if (IsActive) return;

        IsActive = true;
        SetUpdated(updatedBy);
    }

    public void IncreaseUsage(Guid? updatedBy = null)
    {
        if (!IsActive)
            throw new InvalidOperationException("Referral code is inactive");

        UsedCount++;
        SetUpdated(updatedBy);
    }
}
