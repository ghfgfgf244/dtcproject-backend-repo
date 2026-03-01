namespace dtc.Domain.Entities.Blogs;

public class Category : BaseEntity
{
    public int CategoryId { get; private set; }
    public string CategoryName { get; private set; } = default!;
    public int? ParentCategoryId { get; private set; }

    public bool SubCategoryStatus { get; private set; }
    public bool ShowMenuStatus { get; private set; }
    public bool Status { get; private set; }

    protected Category() { }

    // =========================
    // CREATE
    // =========================
    public Category(
        string name,
        Guid? createdBy,
        int? parentCategoryId = null)
    {
        SetName(name);
        SetParent(parentCategoryId);

        ShowMenuStatus = true;
        Status = true;

        SetCreated(createdBy);
    }

    // =========================
    // UPDATE
    // =========================
    public bool Update(
        string? name,
        int? parentCategoryId,
        bool? showMenuStatus,
        Guid? updatedBy)
    {
        bool changed = false;

        if (!string.IsNullOrWhiteSpace(name) && CategoryName != name.Trim())
        {
            CategoryName = name.Trim();
            changed = true;
        }

        if (parentCategoryId != ParentCategoryId)
        {
            SetParent(parentCategoryId);
            changed = true;
        }

        if (showMenuStatus.HasValue && ShowMenuStatus != showMenuStatus.Value)
        {
            ShowMenuStatus = showMenuStatus.Value;
            changed = true;
        }

        if (!changed)
            return false;

        SetUpdated(updatedBy);
        return true;
    }

    // =========================
    // STATUS
    // =========================
    public void Disable(Guid? updatedBy)
    {
        if (!Status) return;

        Status = false;
        SetUpdated(updatedBy);
    }

    public void Enable(Guid? updatedBy)
    {
        if (Status) return;

        Status = true;
        SetUpdated(updatedBy);
    }

    // =========================
    // PRIVATE RULES
    // =========================
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));

        CategoryName = name.Trim();
    }

    private void SetParent(int? parentCategoryId)
    {
        ParentCategoryId = parentCategoryId;
        SubCategoryStatus = parentCategoryId.HasValue;
    }
}
